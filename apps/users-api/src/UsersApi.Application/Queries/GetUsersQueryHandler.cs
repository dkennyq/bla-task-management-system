using UsersApi.Application.DTOs;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Queries;

public class GetUsersQueryHandler : IGetUsersQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResponse<UserListItemDto>> Handle(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var users = await _userRepository.GetAllAsync(page, pageSize, cancellationToken);
        var totalCount = await _userRepository.GetCountAsync(cancellationToken);

        return new PagedResponse<UserListItemDto>
        {
            Items = users.Select(u => new UserListItemDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role.ToString(),
                IsActive = true,
                CreatedAt = u.CreatedAt
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
