using System.ComponentModel.DataAnnotations;

namespace UsersApi.Application.Commands;

public class UpdateUserCommand
{
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_.@-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, hyphens, dots, and at signs")]
    public string? Username { get; set; }

    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    public string? CurrentPassword { get; set; }

    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string? NewPassword { get; set; }
}
