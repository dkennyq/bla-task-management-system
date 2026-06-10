using UsersApi.Domain.Entities;

namespace UsersApi.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
