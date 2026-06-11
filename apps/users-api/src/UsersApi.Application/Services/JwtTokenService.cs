using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UsersApi.Domain.Entities;

namespace UsersApi.Application.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(UserEntity user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(_settings.ExpirationHours);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.GetEffectiveSecretKey()));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;

    public string? Secret { get; set; }

    public string Issuer { get; set; } = "TaskManagementAPI";
    public string Audience { get; set; } = "TaskManagementWeb";
    public int ExpirationHours { get; set; } = 24;

    public string GetEffectiveSecretKey() => Secret ?? SecretKey;
}
