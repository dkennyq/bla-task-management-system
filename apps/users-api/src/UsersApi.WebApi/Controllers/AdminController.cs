using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApi.Application.Commands;
using UsersApi.Application.DTOs;
using UsersApi.Application.Exceptions;
using UsersApi.WebApi.DTOs;

namespace UsersApi.WebApi.Controllers;

[Authorize(Roles = "Manager")]
[ApiController]
[Route("api/users/admin")]
public class AdminController : ControllerBase
{
    private readonly ICreateUserByAdminCommandHandler _createUserHandler;
    private readonly IUpdateUserRoleCommandHandler _updateUserRoleHandler;

    public AdminController(
        ICreateUserByAdminCommandHandler createUserHandler,
        IUpdateUserRoleCommandHandler updateUserRoleHandler)
    {
        _createUserHandler = createUserHandler;
        _updateUserRoleHandler = updateUserRoleHandler;
    }

    [HttpPost("create")]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserByAdminCommand command)
    {
        try
        {
            var response = await _createUserHandler.Handle(command);
            return CreatedAtAction(nameof(CreateUser), null, response);
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

    [HttpPut("{id}/role")]
    public async Task<ActionResult<UserDto>> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var requestingUserId))
            return Unauthorized(new { message = "Invalid token" });

        var command = new UpdateUserRoleCommand
        {
            UserId = id,
            RequestingUserId = requestingUserId,
            Role = request.Role
        };

        try
        {
            var response = await _updateUserRoleHandler.Handle(command);
            return Ok(response);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
