using Microsoft.AspNetCore.Identity;

namespace LogiTrack.Models;

/// <summary>
/// Usuario personalizado para Identity con propiedades adicionales
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}