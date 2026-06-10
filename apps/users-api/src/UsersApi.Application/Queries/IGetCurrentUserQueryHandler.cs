using UsersApi.Application.DTOs;

namespace UsersApi.Application.Queries;

public interface IGetCurrentUserQueryHandler
{
    Task<UserDto> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken = default);
}
