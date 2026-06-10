using FluentAssertions;
using Moq;
using UsersApi.Application.DTOs;
using UsersApi.Application.Queries;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Tests.Queries;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new GetUsersQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedUsers_WhenUsersExist()
    {
        var users = new List<UserEntity>
        {
            new(Guid.NewGuid(), "alice", "Alice Smith", "alice@example.com", "hash1"),
            new(Guid.NewGuid(), "bob", "Bob Jones", "bob@example.com", "hash2")
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _repositoryMock
            .Setup(r => r.GetCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var query = new GetUsersQuery { Page = 1, PageSize = 10 };
        var result = await _handler.Handle(query);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items[0].Username.Should().Be("alice");
        result.Items[0].FullName.Should().Be("Alice Smith");
        result.Items[1].Username.Should().Be("bob");
        result.Items[1].FullName.Should().Be("Bob Jones");
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldNotExposeSensitiveFields()
    {
        var users = new List<UserEntity>
        {
            new(Guid.NewGuid(), "alice", "Alice Smith", "alice@example.com", "hash1")
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        _repositoryMock
            .Setup(r => r.GetCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var query = new GetUsersQuery();
        var result = await _handler.Handle(query);

        result.Items[0].Should().NotBeNull();
        typeof(UserListItemDto).GetProperty("Email").Should().BeNull();
        typeof(UserListItemDto).GetProperty("PasswordHash").Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldCapPageSize_AtMaximumOf50()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserEntity>());

        _repositoryMock
            .Setup(r => r.GetCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var query = new GetUsersQuery { Page = 1, PageSize = 100 };
        var result = await _handler.Handle(query);

        result.PageSize.Should().Be(50);
        _repositoryMock.Verify(r => r.GetAllAsync(1, 50, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldDefaultPage_ToAtLeast1()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserEntity>());

        _repositoryMock
            .Setup(r => r.GetCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var query = new GetUsersQuery { Page = 0, PageSize = 10 };
        var result = await _handler.Handle(query);

        result.Page.Should().Be(1);
        _repositoryMock.Verify(r => r.GetAllAsync(1, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsers()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserEntity>());

        _repositoryMock
            .Setup(r => r.GetCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var query = new GetUsersQuery();
        var result = await _handler.Handle(query);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
