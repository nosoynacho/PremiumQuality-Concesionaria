using System.ComponentModel.DataAnnotations;

namespace blazornacho.Models;

public class LoginUsuarioModel
{
    [Required(
        ErrorMessage = "Ingresá tu correo electrónico."
    )]
    [EmailAddress(
        ErrorMessage = "Ingresá un correo electrónico válido."
    )]
    public string Correo { get; set; } = "";

    [Required(
        ErrorMessage = "Ingresá tu contraseña."
    )]
    public string Password { get; set; } = "";
}