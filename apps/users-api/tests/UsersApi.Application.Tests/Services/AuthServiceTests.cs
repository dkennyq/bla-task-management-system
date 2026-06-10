using FluentAssertions;
using Moq;
using UsersApi.Application.DTOs;
using UsersApi.Application.Services;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _authService = new AuthService(
            _userRepoMock.Object,
            _refreshTokenRepoMock.Object,
            _jwtTokenServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnLoginResponse_WhenEmailNotTaken()
    {
        var request = new RegisterRequest
        {
            FullName = "Test User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _userRepoMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        _jwtTokenServiceMock
            .Setup(s => s.GenerateToken(It.IsAny<UserEntity>()))
            .Returns(("jwt-token", DateTime.UtcNow.AddHours(24)));

        _refreshTokenRepoMock
            .Setup(r => r.RevokeAllForUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _refreshTokenRepoMock
            .Setup(r => r.AddAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(request);

        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token");
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(request.Email);
        result.FullName.Should().Be(request.FullName);

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokenRepoMock.Verify(r => r.RevokeAllForUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _refreshTokenRepoMock.Verify(r => r.AddAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrow_WhenEmailTaken()
    {
        var request = new RegisterRequest
        {
            FullName = "Test User",
            Email = "existing@example.com",
            Password = "Password123!"
        };

        _userRepoMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserEntity(Guid.NewGuid(), "Existing", request.Email, "hash"));

        Func<Task> act = async () => await _authService.RegisterAsync(request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email is already registered");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnLoginResponse_WhenCredentialsValid()
    {
        var userId = Guid.NewGuid();
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var user = new UserEntity(userId, "Test User", request.Email,
            BCrypt.Net.BCrypt.HashPassword(request.Password));

        _userRepoMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtTokenServiceMock
            .Setup(s => s.GenerateToken(user))
            .Returns(("jwt-token", DateTime.UtcNow.AddHours(24)));

        _refreshTokenRepoMock
            .Setup(r => r.RevokeAllForUserAsync(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _refreshTokenRepoMock
            .Setup(r => r.AddAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _authService.LoginAsync(request);

        result.Should().NotBeNull();
        result.Token.Should().Be("jwt-token");
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenEmailNotFound()
    {
        var request = new LoginRequest
        {
            Email = "unknown@example.com",
            Password = "Password123!"
        };

        _userRepoMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        Func<Task> act = async () => await _authService.LoginAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrow_WhenPasswordWrong()
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        var user = new UserEntity(Guid.NewGuid(), "Test User", request.Email,
            BCrypt.Net.BCrypt.HashPassword("CorrectPassword"));

        _userRepoMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        Func<Task> act = async () => await _authService.LoginAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenTokenValid()
    {
        var userId = Guid.NewGuid();
        var refreshTokenValue = "valid-refresh-token";

        var storedToken = new RefreshTokenEntity(
            Guid.NewGuid(), userId, refreshTokenValue, DateTime.UtcNow.AddDays(7));

        var user = new UserEntity(userId, "Test User", "test@example.com", "hash");

        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync(refreshTokenValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);

        _userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _jwtTokenServiceMock
            .Setup(s => s.GenerateToken(user))
            .Returns(("new-jwt-token", DateTime.UtcNow.AddHours(24)));

        _refreshTokenRepoMock
            .Setup(r => r.RevokeAllForUserAsync(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _refreshTokenRepoMock
            .Setup(r => r.AddAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var request = new RefreshTokenRequest { RefreshToken = refreshTokenValue };
        var result = await _authService.RefreshTokenAsync(request);

        result.Should().NotBeNull();
        result.Token.Should().Be("new-jwt-token");
        result.RefreshToken.Should().NotBeNullOrEmpty();
        storedToken.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenTokenNotFound()
    {
        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync("invalid-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshTokenEntity?)null);

        var request = new RefreshTokenRequest { RefreshToken = "invalid-token" };
        Func<Task> act = async () => await _authService.RefreshTokenAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid refresh token");
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenTokenExpired()
    {
        var storedToken = new RefreshTokenEntity(
            Guid.NewGuid(), Guid.NewGuid(), "expired-token", DateTime.UtcNow.AddDays(-1));

        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync("expired-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);

        var request = new RefreshTokenRequest { RefreshToken = "expired-token" };
        Func<Task> act = async () => await _authService.RefreshTokenAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*expired*");
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldThrow_WhenTokenRevoked()
    {
        var storedToken = new RefreshTokenEntity(
            Guid.NewGuid(), Guid.NewGuid(), "revoked-token", DateTime.UtcNow.AddDays(7));
        storedToken.Revoke();

        _refreshTokenRepoMock
            .Setup(r => r.GetByTokenAsync("revoked-token", It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);

        var request = new RefreshTokenRequest { RefreshToken = "revoked-token" };
        Func<Task> act = async () => await _authService.RefreshTokenAsync(request);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*revoked*");
    }
}
