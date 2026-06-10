using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UsersApi.Application.Commands;
using UsersApi.Application.DTOs;
using UsersApi.Application.Exceptions;
using UsersApi.Application.Services;
using UsersApi.WebApi.Controllers;

namespace UsersApi.WebApi.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IRegisterUserCommandHandler> _registerHandlerMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _registerHandlerMock = new Mock<IRegisterUserCommandHandler>();
        _controller = new UsersController(_authServiceMock.Object, _registerHandlerMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturn201_WhenSuccessful()
    {
        var command = new RegisterUserCommand
        {
            Username = "testuser",
            FullName = "Test User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        var response = new UserDto
        {
            Id = Guid.NewGuid(),
            Username = command.Username,
            FullName = command.FullName,
            Email = command.Email,
            CreatedAt = DateTime.UtcNow
        };

        _registerHandlerMock
            .Setup(h => h.Handle(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.Register(command);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be("GetMe");
        createdResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task Register_ShouldReturn409_WhenUsernameTaken()
    {
        var command = new RegisterUserCommand
        {
            Username = "takenuser",
            FullName = "Test User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _registerHandlerMock
            .Setup(h => h.Handle(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflictException("Username 'takenuser' is already taken"));

        var result = await _controller.Register(command);

        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Register_ShouldReturn409_WhenEmailTaken()
    {
        var command = new RegisterUserCommand
        {
            Username = "testuser",
            FullName = "Test User",
            Email = "taken@example.com",
            Password = "Password123!"
        };

        _registerHandlerMock
            .Setup(h => h.Handle(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflictException("Email 'taken@example.com' is already registered"));

        var result = await _controller.Register(command);

        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Register_ShouldReturn400_WhenPasswordTooWeak()
    {
        var command = new RegisterUserCommand
        {
            Username = "testuser",
            FullName = "Test User",
            Email = "test@example.com",
            Password = "weak"
        };

        _registerHandlerMock
            .Setup(h => h.Handle(command, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Password must be at least 8 characters", "password"));

        var result = await _controller.Register(command);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturn200_WhenCredentialsValid()
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var response = new LoginResponse
        {
            UserId = Guid.NewGuid(),
            Email = request.Email,
            FullName = "Test User",
            Token = "jwt-token",
            RefreshToken = "refresh-token",
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        _authServiceMock
            .Setup(s => s.LoginAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.Login(request);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task Login_ShouldReturn401_WhenCredentialsInvalid()
    {
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        _authServiceMock
            .Setup(s => s.LoginAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password"));

        var result = await _controller.Login(request);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task RefreshToken_ShouldReturn200_WhenTokenValid()
    {
        var request = new RefreshTokenRequest { RefreshToken = "valid-refresh-token" };
        var response = new LoginResponse
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            Token = "new-jwt-token",
            RefreshToken = "new-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        _authServiceMock
            .Setup(s => s.RefreshTokenAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.RefreshToken(request);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturn401_WhenTokenInvalid()
    {
        var request = new RefreshTokenRequest { RefreshToken = "invalid-token" };

        _authServiceMock
            .Setup(s => s.RefreshTokenAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid refresh token"));

        var result = await _controller.RefreshToken(request);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetMe_ShouldReturn200_WhenAuthenticated()
    {
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Name, "Test User")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await _controller.GetMe();

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as LoginResponse;
        response!.UserId.Should().Be(userId);
        response.Email.Should().Be("test@example.com");
        response.FullName.Should().Be("Test User");
    }

    [Fact]
    public async Task GetMe_ShouldReturn401_WhenNotAuthenticated()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        var result = await _controller.GetMe();

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
