using System.ComponentModel.DataAnnotations;

namespace UsersApi.Application.Commands;

public class UpdateUserRoleCommand
{
    public Guid UserId { get; set; }
    public Guid RequestingUserId { get; set; }

    [Required(ErrorMessage = "Role is required")]
    [RegularExpression(@"^(Manager|Operator)$", ErrorMessage = "Role must be either 'Manager' or 'Operator'")]
    public string Role { get; set; } = string.Empty;
}
