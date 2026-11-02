using System.ComponentModel.DataAnnotations;

namespace blazornacho.Models;

public class Resena
{
    public long Id { get; set; }


    [Required]
    public string VehiculoId { get; set; } = "";


    public long UsuarioId { get; set; }


    [Range(
        1,
        5,
        ErrorMessage =
            "El puntaje debe estar entre 1 y 5."
    )]
    public int Puntaje { get; set; } = 5;


    [Required]
    [MaxLength(1000)]
    public string Comentario { get; set; } = "";


    public DateTime Fecha { get; set; }
        = DateTime.Now;


    // =====================================================
    // PROPIEDADES DE NAVEGACIÓN
    // =====================================================

    public Vehiculo? Vehiculo { get; set; }

    public Usuario? Usuario { get; set; }
}