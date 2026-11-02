using System.ComponentModel.DataAnnotations;

namespace blazornacho.Models;

public class Usuario
{
    public long Id { get; set; }


    [Required]
    [MaxLength(60)]
    public string Nombre { get; set; } = "";


    [Required]
    [MaxLength(60)]
    public string Apellido { get; set; } = "";


    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Correo { get; set; } = "";


    [Required]
    public string PasswordHash { get; set; } = "";


    [Required]
    public string CodigoRecuperacionHash
        { get; set; } = "";


    public DateTime FechaRegistro { get; set; }
        = DateTime.Now;


    public bool Activo { get; set; } = true;


    // =====================================================
    // RELACIONES
    // =====================================================

    public ICollection<Turno> Turnos
        { get; set; } =
            new List<Turno>();


    public ICollection<Reclamo> Reclamos
        { get; set; } =
            new List<Reclamo>();


    public ICollection<Resena> Resenas
        { get; set; } =
            new List<Resena>();


    public string NombreCompleto =>
        $"{Nombre} {Apellido}".Trim();
}