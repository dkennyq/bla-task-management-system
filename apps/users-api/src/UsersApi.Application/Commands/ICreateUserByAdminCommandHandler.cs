using UsersApi.Application.DTOs;

namespace UsersApi.Application.Commands;

public interface ICreateUserByAdminCommandHandler
{
    Task<UserDto> Handle(CreateUserByAdminCommand command, CancellationToken cancellationToken = default);
}
