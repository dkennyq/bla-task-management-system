using System.ComponentModel.DataAnnotations;

namespace UsersApi.Application.Commands;

public class CreateUserByAdminCommand
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_.@-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, hyphens, dots, and at signs")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [RegularExpression(@"^(Manager|Operator)$", ErrorMessage = "Role must be either 'Manager' or 'Operator'")]
    public string Role { get; set; } = "Operator";
}
