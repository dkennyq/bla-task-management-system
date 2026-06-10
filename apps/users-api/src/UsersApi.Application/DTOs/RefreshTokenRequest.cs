using System.ComponentModel.DataAnnotations;

namespace UsersApi.Application.DTOs;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
