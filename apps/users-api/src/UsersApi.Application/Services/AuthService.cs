using System.Security.Cryptography;
using UsersApi.Application.DTOs;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;

    private static readonly TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(7);

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException("Email is already registered");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new UserEntity(Guid.NewGuid(), request.Username, request.FullName, request.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        return await GenerateLoginResponseAsync(user, cancellationToken);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        return await GenerateLoginResponseAsync(user, cancellationToken);
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken is null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (!storedToken.IsActive)
            throw new UnauthorizedAccessException("Refresh token has expired or has been revoked");

        var user = await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken);
        if (user is null)
            throw new UnauthorizedAccessException("User not found");

        // Revoke old refresh token
        storedToken.Revoke();
        await _refreshTokenRepository.RevokeAllForUserAsync(storedToken.UserId, cancellationToken);

        return await GenerateLoginResponseAsync(user, cancellationToken);
    }

    private async Task<LoginResponse> GenerateLoginResponseAsync(UserEntity user, CancellationToken cancellationToken)
    {
        var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

        // Revoke previous refresh tokens for this user
        await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);

        // Generate new refresh token
        var refreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshToken = new RefreshTokenEntity(
            Guid.NewGuid(),
            user.Id,
            refreshTokenValue,
            DateTime.UtcNow.Add(RefreshTokenExpiry)
        );

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token,
            RefreshToken = refreshTokenValue,
            ExpiresAt = expiresAt
        };
    }
}
