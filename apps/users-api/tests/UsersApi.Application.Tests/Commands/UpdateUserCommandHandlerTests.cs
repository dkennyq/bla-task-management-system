using FluentAssertions;
using Moq;
using UsersApi.Application.Commands;
using UsersApi.Application.Exceptions;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Tests.Commands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UpdateUserCommandHandler _handler;
    private readonly UserEntity _existingUser;

    public UpdateUserCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new UpdateUserCommandHandler(_repositoryMock.Object);

        _existingUser = new UserEntity(
            Guid.NewGuid(),
            "johndoe",
            "John Doe",
            "john@example.com",
            BCrypt.Net.BCrypt.HashPassword("CurrentPass123!", 12)
        );
    }

    [Fact]
    public async Task Handle_ShouldUpdateUsername_WhenValid()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            Username = "newusername"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        _repositoryMock
            .Setup(r => r.ExistsByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(userId, command);

        result.Should().NotBeNull();
        result.Username.Should().Be("newusername");
        result.FullName.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");

        _repositoryMock.Verify(r => r.ExistsByUsernameAsync("newusername", It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateEmail_WhenValid()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            Email = "newemail@example.com"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(userId, command);

        result.Should().NotBeNull();
        result.Email.Should().Be("newemail@example.com");

        _repositoryMock.Verify(r => r.ExistsByEmailAsync("newemail@example.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateFullName_WhenValid()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            FullName = "New Name"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(userId, command);

        result.Should().NotBeNull();
        result.FullName.Should().Be("New Name");
    }

    [Fact]
    public async Task Handle_ShouldUpdatePassword_WhenCurrentPasswordValid()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            CurrentPassword = "CurrentPass123!",
            NewPassword = "NewSecurePass456!"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(userId, command);

        result.Should().NotBeNull();
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        var command = new UpdateUserCommand
        {
            Username = "newuser"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        Func<Task> act = async () => await _handler.Handle(userId, command);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*'{userId}'*");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUsernameTaken()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            Username = "takenuser"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        _repositoryMock
            .Setup(r => r.ExistsByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Func<Task> act = async () => await _handler.Handle(userId, command);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already taken*");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenEmailTaken()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            Email = "taken@example.com"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        _repositoryMock
            .Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        Func<Task> act = async () => await _handler.Handle(userId, command);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already registered*");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCurrentPasswordMissing()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            NewPassword = "NewSecurePass456!"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        Func<Task> act = async () => await _handler.Handle(userId, command);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Current password is required*");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCurrentPasswordIncorrect()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            CurrentPassword = "WrongPassword123!",
            NewPassword = "NewSecurePass456!"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        Func<Task> act = async () => await _handler.Handle(userId, command);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*Current password is incorrect*");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNewPasswordMissing()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            CurrentPassword = "CurrentPass123!"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        Func<Task> act = async () => await _handler.Handle(userId, command);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*New password is required*");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenNoChanges()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        Func<Task> act = async () => await _handler.Handle(userId, command);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*No changes detected*");
    }

    [Fact]
    public async Task Handle_ShouldNotCheckUsernameUniqueness_WhenUsernameUnchanged()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            FullName = "New Full Name"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(userId, command);

        _repositoryMock.Verify(r => r.ExistsByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotCheckEmailUniqueness_WhenEmailUnchanged()
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            FullName = "New Full Name"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _handler.Handle(userId, command);

        _repositoryMock.Verify(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("short", "Password must be at least 8 characters")]
    [InlineData("nouppercase1!", "uppercase letter")]
    [InlineData("NOLOWERCASE1!", "lowercase letter")]
    [InlineData("NoDigits!!", "number")]
    [InlineData("NoSpecialChar1", "special character")]
    public async Task Handle_ShouldThrow_WhenNewPasswordInvalid(string password, string expectedMessage)
    {
        var userId = _existingUser.Id;
        var command = new UpdateUserCommand
        {
            CurrentPassword = "CurrentPass123!",
            NewPassword = password
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        Func<Task> act = async () => await _handler.Handle(userId, command);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"*{expectedMessage}*");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
