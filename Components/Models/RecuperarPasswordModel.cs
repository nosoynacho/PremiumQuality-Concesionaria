using System.ComponentModel.DataAnnotations;

namespace blazornacho.Models;

public class RecuperarPasswordModel
{
    [Required(
        ErrorMessage = "Ingresá tu correo electrónico."
    )]
    [EmailAddress(
        ErrorMessage =
            "Ingresá un correo electrónico válido."
    )]
    public string Correo { get; set; } = "";

    [Required(
        ErrorMessage =
            "Ingresá tu PIN de recuperación."
    )]
    [RegularExpression(
        @"^\d{6}$",
        ErrorMessage =
            "El PIN debe tener exactamente 6 números."
    )]
    public string CodigoRecuperacion { get; set; } = "";

    [Required(
        ErrorMessage =
            "Ingresá la nueva contraseña."
    )]
    [StringLength(
        100,
        MinimumLength = 8,
        ErrorMessage =
            "La contraseña debe tener al menos 8 caracteres."
    )]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,100}$",
        ErrorMessage =
            "La contraseña debe incluir al menos una mayúscula, una minúscula y un número."
    )]
    public string NuevaPassword { get; set; } = "";

    [Required(
        ErrorMessage =
            "Repetí la nueva contraseña."
    )]
    [Compare(
        nameof(NuevaPassword),
        ErrorMessage =
            "Las contraseñas no coinciden."
    )]
    public string ConfirmarPassword { get; set; } = "";
}