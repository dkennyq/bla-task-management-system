using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApi.Application.DTOs;
using UsersApi.Application.Services;

namespace UsersApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(GetMe), null, response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<LoginResponse>> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid token" });

        // User info is already in claims, return them
        return Ok(new LoginResponse
        {
            UserId = userId,
            Email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            FullName = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            Token = string.Empty,
            RefreshToken = string.Empty,
            ExpiresAt = DateTime.MinValue
        });
    }
}
