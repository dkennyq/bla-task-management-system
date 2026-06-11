using System.ComponentModel.DataAnnotations;

namespace UsersApi.WebApi.DTOs;

public class UpdateUserRoleRequest
{
    [Required(ErrorMessage = "Role is required")]
    [RegularExpression(@"^(Manager|Operator)$", ErrorMessage = "Role must be either 'Manager' or 'Operator'")]
    public string Role { get; set; } = string.Empty;
}
