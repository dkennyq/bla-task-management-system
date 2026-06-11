using System.ComponentModel.DataAnnotations;

namespace UsersApi.Application.Commands;

public class ResetPasswordCommand
{
    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string NewPassword { get; set; } = string.Empty;
}
