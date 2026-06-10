using System.Text.RegularExpressions;

namespace UsersApi.Domain.ValueObjects;

public partial class Username
{
    private static readonly Regex UsernameRegex = ValidUsernamePattern();

    public string Value { get; }

    public Username(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Username cannot be empty", nameof(value));

        if (value.Length < 3)
            throw new ArgumentException("Username must be at least 3 characters", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Username cannot exceed 50 characters", nameof(value));

        if (!UsernameRegex.IsMatch(value))
            throw new ArgumentException("Username can only contain letters, numbers, underscores, and hyphens", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-zA-Z0-9_-]+$")]
    private static partial Regex ValidUsernamePattern();
}
