using UsersApi.Application.DTOs;
using UsersApi.Application.Exceptions;
using UsersApi.Domain.Entities;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Commands;

public class RegisterUserCommandHandler : IRegisterUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private const int BcryptWorkFactor = 12;

    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        ValidatePasswordComplexity(command.Password);

        if (await _userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
            throw new ConflictException($"Username '{command.Username}' is already taken");

        if (await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
            throw new ConflictException($"Email '{command.Email}' is already registered");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(command.Password, BcryptWorkFactor);
        var user = new UserEntity(Guid.NewGuid(), command.Username, command.FullName, command.Email, passwordHash);

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    private static void ValidatePasswordComplexity(string password)
    {
        if (password.Length < 8)
            throw new ArgumentException("Password must be at least 8 characters", nameof(password));

        if (!password.Any(char.IsUpper))
            throw new ArgumentException("Password must contain at least one uppercase letter", nameof(password));

        if (!password.Any(char.IsLower))
            throw new ArgumentException("Password must contain at least one lowercase letter", nameof(password));

        if (!password.Any(char.IsDigit))
            throw new ArgumentException("Password must contain at least one number", nameof(password));

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            throw new ArgumentException("Password must contain at least one special character", nameof(password));
    }
}
