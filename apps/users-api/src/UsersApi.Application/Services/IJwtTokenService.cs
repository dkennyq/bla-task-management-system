using UsersApi.Domain.Entities;

namespace UsersApi.Application.Services;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(UserEntity user);
}
