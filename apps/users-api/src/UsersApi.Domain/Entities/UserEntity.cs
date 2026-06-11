using UsersApi.Domain.Enums;
using UsersApi.Domain.ValueObjects;

namespace UsersApi.Domain.Entities;

public class UserEntity
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public UserEntity(Guid id, string username, string fullName, string email, string passwordHash, UserRole role = UserRole.Operator)
    {
        var usernameVo = new Username(username);

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        var emailVo = new Email(email);

        if (!Enum.IsDefined(typeof(UserRole), role))
            throw new ArgumentException("Invalid role", nameof(role));

        Id = id;
        Username = usernameVo.Value;
        FullName = fullName;
        Email = emailVo.Value;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string? username, string? fullName, string? email)
    {
        if (!string.IsNullOrWhiteSpace(username))
            Username = new Username(username).Value;

        if (!string.IsNullOrWhiteSpace(fullName))
            FullName = fullName;

        if (!string.IsNullOrWhiteSpace(email))
            Email = new Email(email).Value;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }

    public void UpdateRole(UserRole newRole)
    {
        if (!Enum.IsDefined(typeof(UserRole), newRole))
            throw new ArgumentException("Invalid role", nameof(newRole));
        Role = newRole;
    }
}
