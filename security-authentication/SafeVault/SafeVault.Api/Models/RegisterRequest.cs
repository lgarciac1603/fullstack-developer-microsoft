using System.ComponentModel.DataAnnotations;

namespace SafeVault.Api.Models;

public class RegisterRequest
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = "";

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = "";
}