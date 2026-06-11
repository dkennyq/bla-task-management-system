using UsersApi.Application.DTOs;

namespace UsersApi.Application.Commands;

public interface IResetPasswordCommandHandler
{
    Task<UserDto> Handle(Guid userId, ResetPasswordCommand command, CancellationToken cancellationToken = default);
}
