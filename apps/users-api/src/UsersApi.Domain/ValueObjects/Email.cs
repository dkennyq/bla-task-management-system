using System.Text.RegularExpressions;

namespace UsersApi.Domain.ValueObjects;

public partial class Email
{
    private static readonly Regex EmailRegex = ValidEmailPattern();

    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        if (!EmailRegex.IsMatch(value))
            throw new ArgumentException("Invalid email format", nameof(value));

        if (value.Length > 255)
            throw new ArgumentException("Email cannot exceed 255 characters", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex ValidEmailPattern();
}
