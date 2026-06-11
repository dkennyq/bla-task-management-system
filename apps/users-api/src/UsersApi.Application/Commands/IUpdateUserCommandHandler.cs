using UsersApi.Application.DTOs;

namespace UsersApi.Application.Commands;

public interface IUpdateUserCommandHandler
{
    Task<UserDto> Handle(Guid userId, UpdateUserCommand command, CancellationToken cancellationToken = default);
}
