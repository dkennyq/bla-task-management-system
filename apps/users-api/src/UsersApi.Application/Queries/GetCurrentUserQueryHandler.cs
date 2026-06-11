using UsersApi.Application.DTOs;
using UsersApi.Application.Exceptions;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Queries;

public class GetCurrentUserQueryHandler : IGetCurrentUserQueryHandler
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException($"User with ID '{query.UserId}' not found");

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
