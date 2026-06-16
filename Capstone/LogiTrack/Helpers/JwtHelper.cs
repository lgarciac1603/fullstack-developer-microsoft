using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using LogiTrack.Models;

namespace LogiTrack.Helpers;

/// <summary>
/// Helper para generar tokens JWT
/// </summary>
public static class JwtHelper
{
    /// <summary>
    /// Genera un token JWT para un usuario
    /// </summary>
    public static string GenerateJwtToken(ApplicationUser user, IConfiguration configuration)
    {
        // Obtener roles del usuario
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        // Agregar claims de roles (se añadirán después en el controlador)
        // ya que aquí no tenemos acceso al UserManager

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ??
                "DefaultSecretKey12345678901234567890"));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"] ?? "LogiTrackAPI",
            audience: configuration["Jwt:Audience"] ?? "LogiTrackClients",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}