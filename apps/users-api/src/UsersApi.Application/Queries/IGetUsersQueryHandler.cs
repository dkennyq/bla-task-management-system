using UsersApi.Application.DTOs;

namespace UsersApi.Application.Queries;

public interface IGetUsersQueryHandler
{
    Task<PagedResponse<UserListItemDto>> Handle(GetUsersQuery query, CancellationToken cancellationToken = default);
}
