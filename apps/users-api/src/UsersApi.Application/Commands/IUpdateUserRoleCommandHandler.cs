using UsersApi.Application.DTOs;

namespace UsersApi.Application.Commands;

public interface IUpdateUserRoleCommandHandler
{
    Task<UserDto> Handle(UpdateUserRoleCommand command, CancellationToken cancellationToken = default);
}
