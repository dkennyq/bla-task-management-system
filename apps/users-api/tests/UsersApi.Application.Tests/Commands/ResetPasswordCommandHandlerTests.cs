using FluentAssertions;
using Moq;
using UsersApi.Application.Commands;
using UsersApi.Application.Exceptions;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Tests.Commands;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly ResetPasswordCommandHandler _handler;
    private readonly UserEntity _existingUser;
    private readonly Guid _userId;

    public ResetPasswordCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new ResetPasswordCommandHandler(_repositoryMock.Object);

        _userId = Guid.NewGuid();
        _existingUser = new UserEntity(
            _userId,
            "johndoe",
            "John Doe",
            "john@example.com",
            BCrypt.Net.BCrypt.HashPassword("OldPass123!", 12)
        );
    }

    [Fact]
    public async Task Handle_ShouldResetPassword_WhenValid()
    {
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePass456!"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(_userId, command);

        result.Should().NotBeNull();
        result.Username.Should().Be("johndoe");
        result.FullName.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        var command = new ResetPasswordCommand { NewPassword = "NewSecurePass456!" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        Func<Task> act = async () => await _handler.Handle(userId, command);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*'{userId}'*");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("short", "Password must be at least 8 characters")]
    [InlineData("nouppercase1!", "uppercase letter")]
    [InlineData("NOLOWERCASE1!", "lowercase letter")]
    [InlineData("NoDigits!!", "number")]
    [InlineData("NoSpecialChar1", "special character")]
    public async Task Handle_ShouldThrow_WhenNewPasswordInvalid(string password, string expectedMessage)
    {
        var command = new ResetPasswordCommand { NewPassword = password };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        Func<Task> act = async () => await _handler.Handle(_userId, command);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"*{expectedMessage}*");

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdatePasswordHash_WhenValid()
    {
        var command = new ResetPasswordCommand
        {
            NewPassword = "NewSecurePass456!"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingUser);

        UserEntity? capturedUser = null;
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()))
            .Callback<UserEntity, CancellationToken>((u, _) => capturedUser = u)
            .Returns(Task.CompletedTask);

        await _handler.Handle(_userId, command);

        capturedUser.Should().NotBeNull();
        BCrypt.Net.BCrypt.Verify("NewSecurePass456!", capturedUser!.PasswordHash).Should().BeTrue();
    }
}
