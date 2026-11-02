using System.ComponentModel.DataAnnotations;

namespace blazornacho.Models;

public class Reclamo
{
    public long Id { get; set; }


    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = "";


    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Correo { get; set; } = "";


    public string Telefono { get; set; } = "";

    public string Seguro { get; set; } = "";

    public string Marca { get; set; } = "";

    public string Modelo { get; set; } = "";

    public string Patente { get; set; } = "";

    public string Siniestro { get; set; } = "";

    public string Mensaje { get; set; } = "";


    public DateTime FechaEnvio { get; set; }
        = DateTime.Now;


    // =====================================================
    // FOREIGN KEY
    // =====================================================

    public long UsuarioId { get; set; }


    public Usuario? Usuario { get; set; }
}