using UsersApi.Application.DTOs;
using UsersApi.Application.Exceptions;
using UsersApi.Domain.Enums;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Commands;

public class UpdateUserRoleCommandHandler : IUpdateUserRoleCommandHandler
{
    private readonly IUserRepository _userRepository;

    public UpdateUserRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(UpdateUserRoleCommand command, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<UserRole>(command.Role, true, out var newRole) || !Enum.IsDefined(typeof(UserRole), newRole))
            throw new ArgumentException("Role must be either 'Manager' or 'Operator'", nameof(command.Role));

        if (command.UserId == command.RequestingUserId)
            throw new InvalidOperationException("You cannot change your own role");

        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException($"User with ID '{command.UserId}' not found");

        if (user.Role == UserRole.Manager && newRole == UserRole.Operator)
        {
            var managerCount = await _userRepository.GetManagerCountAsync(cancellationToken);
            if (managerCount <= 1)
                throw new InvalidOperationException("Cannot downgrade the last Manager. At least one Manager must exist in the system.");
        }

        user.UpdateRole(newRole);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            CreatedAt = user.CreatedAt
        };
    }
}
