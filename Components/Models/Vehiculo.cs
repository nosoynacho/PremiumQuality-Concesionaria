using System.ComponentModel.DataAnnotations;

namespace blazornacho.Models;

public class Vehiculo
{
    [Key]
    [MaxLength(40)]
    public string Id { get; set; } = "";


    [Required]
    [MaxLength(60)]
    public string Marca { get; set; } = "";


    [Required]
    [MaxLength(100)]
    public string Modelo { get; set; } = "";


    [Required]
    [MaxLength(10)]
    public string Anio { get; set; } = "";


    public string Motor { get; set; } = "";

    public string Potencia { get; set; } = "";

    public string Transmision { get; set; } = "";

    public string Traccion { get; set; } = "";

    public string Asientos { get; set; } = "";

    public string Color { get; set; } = "";

    public decimal Precio { get; set; }

    public string Segmento { get; set; } = "";

    public string Imagen { get; set; } = "";


    // Vehículo 1 ----- N Reseñas
    public ICollection<Resena> Resenas
        { get; set; } =
            new List<Resena>();
}