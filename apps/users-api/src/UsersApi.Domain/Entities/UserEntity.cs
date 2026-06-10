using UsersApi.Domain.ValueObjects;

namespace UsersApi.Domain.Entities;

public class UserEntity
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public UserEntity(Guid id, string username, string fullName, string email, string passwordHash)
    {
        var usernameVo = new Username(username);

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        var emailVo = new Email(email);

        Id = id;
        Username = usernameVo.Value;
        FullName = fullName;
        Email = emailVo.Value;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }
}
