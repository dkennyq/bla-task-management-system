using UsersApi.Application.DTOs;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException("Email is already registered");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new UserEntity(Guid.NewGuid(), request.FullName, request.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token,
            ExpiresAt = expiresAt
        };
    }
}
