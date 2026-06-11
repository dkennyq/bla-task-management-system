using UsersApi.Application.DTOs;
using UsersApi.Application.Exceptions;
using UsersApi.Domain.Interfaces;

namespace UsersApi.Application.Commands;

public class UpdateUserCommandHandler : IUpdateUserCommandHandler
{
    private readonly IUserRepository _userRepository;
    private const int BcryptWorkFactor = 12;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> Handle(Guid userId, UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"User with ID '{userId}' not found");

        var hasChanges = false;

        if (command.Username is not null && command.Username != user.Username)
        {
            if (await _userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
                throw new ConflictException($"Username '{command.Username}' is already taken");
            hasChanges = true;
        }

        if (command.Email is not null && command.Email != user.Email)
        {
            if (await _userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
                throw new ConflictException($"Email '{command.Email}' is already registered");
            hasChanges = true;
        }

        if (command.FullName is not null && command.FullName != user.FullName)
            hasChanges = true;

        if (command.NewPassword is not null)
        {
            if (string.IsNullOrWhiteSpace(command.CurrentPassword))
                throw new ArgumentException("Current password is required when setting a new password", nameof(command.CurrentPassword));

            if (!BCrypt.Net.BCrypt.Verify(command.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect");

            ValidatePasswordComplexity(command.NewPassword);
            hasChanges = true;
        }
        else if (command.CurrentPassword is not null)
        {
            throw new ArgumentException("New password is required when providing current password", nameof(command.NewPassword));
        }

        if (!hasChanges)
            throw new ArgumentException("No changes detected. Provide at least one field to update.", nameof(command));

        user.UpdateProfile(command.Username, command.FullName, command.Email);

        if (command.NewPassword is not null)
        {
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(command.NewPassword, BcryptWorkFactor);
            user.UpdatePassword(newPasswordHash);
        }

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
