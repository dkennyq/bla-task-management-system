using UsersApi.Application.DTOs;

namespace UsersApi.Application.Commands;

public interface IRegisterUserCommandHandler
{
    Task<UserDto> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default);
}
