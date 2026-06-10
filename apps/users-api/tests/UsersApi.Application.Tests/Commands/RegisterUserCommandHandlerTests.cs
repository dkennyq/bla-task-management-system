using FluentAssertions;
using Moq;
using UsersApi.Application.Commands;
using UsersApi.Application.Exceptions;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Tests.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new RegisterUserCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValid()
    {
        var command = new RegisterUserCommand
        {
            Username = "johndoe",
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "SecurePass123!"
        };

        _repositoryMock
            .Setup(r => r.ExistsByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command);

        result.Should().NotBeNull();
        result.Username.Should().Be(command.Username);
        result.FullName.Should().Be(command.FullName);
        result.Email.Should().Be(command.Email);
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _repositoryMock.Verify(r => r.ExistsByUsernameAsync(command.Username, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUsernameTaken()
    {
        var command = new RegisterUserCommand
        {
            Username = "takenuser",
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "SecurePass123!"
        };

        _repositoryMock
            .Setup(r => r.ExistsByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Func<Task> act = async () => await _handler.Handle(command);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already taken*");

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenEmailTaken()
    {
        var command = new RegisterUserCommand
        {
            Username = "johndoe",
            FullName = "John Doe",
            Email = "taken@example.com",
            Password = "SecurePass123!"
        };

        _repositoryMock
            .Setup(r => r.ExistsByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Func<Task> act = async () => await _handler.Handle(command);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already registered*");

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("short", "Password must be at least 8 characters")]
    [InlineData("nouppercase1!", "uppercase letter")]
    [InlineData("NOLOWERCASE1!", "lowercase letter")]
    [InlineData("NoDigits!!", "number")]
    [InlineData("NoSpecialChar1", "special character")]
    public async Task Handle_ShouldThrow_WhenPasswordInvalid(string password, string expectedMessage)
    {
        var command = new RegisterUserCommand
        {
            Username = "johndoe",
            FullName = "John Doe",
            Email = "john@example.com",
            Password = password
        };

        _repositoryMock
            .Setup(r => r.ExistsByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        Func<Task> act = async () => await _handler.Handle(command);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"*{expectedMessage}*");

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCheckEmailUniqueness_OnlyAfterUsername()
    {
        var command = new RegisterUserCommand
        {
            Username = "takenuser",
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "SecurePass123!"
        };

        _repositoryMock
            .Setup(r => r.ExistsByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Func<Task> act = async () => await _handler.Handle(command);

        await act.Should().ThrowAsync<ConflictException>();

        _repositoryMock.Verify(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
