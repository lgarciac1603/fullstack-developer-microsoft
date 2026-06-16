using System.ComponentModel.DataAnnotations;

namespace LogiTrack.DTOs;

/// <summary>
/// Data Transfer Object para login de usuarios
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}