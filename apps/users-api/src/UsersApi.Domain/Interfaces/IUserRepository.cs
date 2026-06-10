using UsersApi.Domain.Entities;

namespace UsersApi.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(UserEntity user, CancellationToken cancellationToken = default);
}
