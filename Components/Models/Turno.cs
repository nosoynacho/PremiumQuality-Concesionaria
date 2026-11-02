using System.ComponentModel.DataAnnotations;

namespace blazornacho.Models;

public class Turno
{
    public long Id { get; set; }


    [Required]
    public DateTime Fecha { get; set; }
        = DateTime.Today;


    [Required]
    [MaxLength(5)]
    public string Hora { get; set; } = "";


    [Required]
    [MaxLength(80)]
    public string Nombre { get; set; } = "";


    [Required]
    [MaxLength(250)]
    public string Motivo { get; set; } = "";


    public DateTime FechaReserva { get; set; }
        = DateTime.Now;


    // =====================================================
    // FOREIGN KEY
    // =====================================================

    public long UsuarioId { get; set; }


    // Propiedad de navegación.
    public Usuario? Usuario { get; set; }
}