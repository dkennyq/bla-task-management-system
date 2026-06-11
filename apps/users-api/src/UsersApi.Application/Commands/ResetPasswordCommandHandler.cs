using UsersApi.Application.DTOs;
using UsersApi.Application.Exceptions;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Commands;

public class ResetPasswordCommandHandler : IResetPasswordCommandHandler
{
    private readonly IUserRepository _userRepository;
    private const int BcryptWorkFactor = 12;

    public ResetPasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(Guid userId, ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User with ID '{userId}' not found");

        ValidatePasswordComplexity(command.NewPassword);

        var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword, BcryptWorkFactor);
        user.UpdatePassword(newPasswordHash);

        await _userRepository.UpdateAsync(user, cancellationToken);

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
