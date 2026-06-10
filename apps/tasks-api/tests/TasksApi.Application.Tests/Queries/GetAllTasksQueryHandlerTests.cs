using FluentAssertions;
using Moq;
using TasksApi.Application.Interfaces;
using TasksApi.Application.Queries;
using TasksApi.Domain.Entities;
using Xunit;

namespace TasksApi.Application.Tests.Queries;

public class GetAllTasksQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _repositoryMock;
    private readonly GetAllTasksQueryHandler _handler;

    public GetAllTasksQueryHandlerTests()
    {
        _repositoryMock = new Mock<ITaskRepository>();
        _handler = new GetAllTasksQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllTasks_WhenTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedTasks = new List<TaskEntity>
        {
            new TaskEntity(
                Guid.NewGuid(),
                "Task 1",
                "Description 1",
                TaskEntityStatus.Pending,
                TaskPriority.High,
                DateTime.UtcNow.AddDays(1),
                userId
            ),
            new TaskEntity(
                Guid.NewGuid(),
                "Task 2",
                "Description 2",
                TaskEntityStatus.InProgress,
                TaskPriority.Medium,
                DateTime.UtcNow.AddDays(2),
                userId
            )
        };

        _repositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTasks);

        var query = new GetAllTasksQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(expectedTasks);
        _repositoryMock.Verify(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyList = new List<TaskEntity>();

        _repositoryMock
            .Setup(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyList);

        var query = new GetAllTasksQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _repositoryMock.Verify(r => r.GetAllByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUserIdIsEmpty()
    {
        // Arrange
        var query = new GetAllTasksQuery(Guid.Empty);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*userId*");
    }
}
