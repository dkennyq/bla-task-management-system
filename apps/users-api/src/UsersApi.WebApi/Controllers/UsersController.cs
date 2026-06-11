using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApi.Application.Commands;
using UsersApi.Application.DTOs;
using UsersApi.Application.Exceptions;
using UsersApi.Application.Queries;
using UsersApi.Application.Services;

namespace UsersApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IRegisterUserCommandHandler _registerHandler;
    private readonly IGetCurrentUserQueryHandler _getCurrentUserHandler;
    private readonly IGetUsersQueryHandler _getUsersHandler;
    private readonly IUpdateUserCommandHandler _updateUserHandler;
    private readonly IResetPasswordCommandHandler _resetPasswordHandler;

    public UsersController(IAuthService authService, IRegisterUserCommandHandler registerHandler, IGetCurrentUserQueryHandler getCurrentUserHandler, IGetUsersQueryHandler getUsersHandler, IUpdateUserCommandHandler updateUserHandler, IResetPasswordCommandHandler resetPasswordHandler)
    {
        _authService = authService;
        _registerHandler = registerHandler;
        _getCurrentUserHandler = getCurrentUserHandler;
        _getUsersHandler = getUsersHandler;
        _updateUserHandler = updateUserHandler;
        _resetPasswordHandler = resetPasswordHandler;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserCommand command)
    {
        try
        {
            var response = await _registerHandler.Handle(command);
            return CreatedAtAction(nameof(GetMe), null, response);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
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

    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserListItemDto>>> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await _getUsersHandler.Handle(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid token" });

        try
        {
            var query = new GetCurrentUserQuery { UserId = userId };
            var user = await _getCurrentUserHandler.Handle(query);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateMe([FromBody] UpdateUserCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid token" });

        try
        {
            var user = await _updateUserHandler.Handle(userId, command);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("me/reset-password")]
    public async Task<ActionResult<UserDto>> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Invalid token" });

        try
        {
            var user = await _resetPasswordHandler.Handle(userId, command);
            return Ok(user);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
