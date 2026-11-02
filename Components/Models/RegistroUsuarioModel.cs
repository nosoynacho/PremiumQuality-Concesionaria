using System.ComponentModel.DataAnnotations;

namespace blazornacho.Models;

public class RegistroUsuarioModel
{
    [Required(
        ErrorMessage = "Ingresá tu nombre."
    )]
    [StringLength(
        60,
        MinimumLength = 2,
        ErrorMessage =
            "El nombre debe tener entre 2 y 60 caracteres."
    )]
    [RegularExpression(
        @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s'-]+$",
        ErrorMessage =
            "El nombre solo puede contener letras y espacios."
    )]
    public string Nombre { get; set; } = "";


    [Required(
        ErrorMessage = "Ingresá tu apellido."
    )]
    [StringLength(
        60,
        MinimumLength = 2,
        ErrorMessage =
            "El apellido debe tener entre 2 y 60 caracteres."
    )]
    [RegularExpression(
        @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s'-]+$",
        ErrorMessage =
            "El apellido solo puede contener letras y espacios."
    )]
    public string Apellido { get; set; } = "";


    [Required(
        ErrorMessage =
            "Ingresá tu correo electrónico."
    )]
    [EmailAddress(
        ErrorMessage =
            "Ingresá un correo electrónico válido."
    )]
    [StringLength(
        150,
        ErrorMessage =
            "El correo electrónico es demasiado largo."
    )]
    public string Correo { get; set; } = "";


    [Required(
        ErrorMessage =
            "Ingresá una contraseña."
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
    public string Password { get; set; } = "";


    [Required(
        ErrorMessage =
            "Repetí la contraseña."
    )]
    [Compare(
        nameof(Password),
        ErrorMessage =
            "Las contraseñas no coinciden."
    )]
    public string ConfirmarPassword { get; set; } = "";


    [Required(
        ErrorMessage =
            "Ingresá tu PIN de recuperación."
    )]
    [RegularExpression(
        @"^\d{6}$",
        ErrorMessage =
            "El PIN de recuperación debe tener exactamente 6 números."
    )]
    public string CodigoRecuperacion { get; set; } = "";
}