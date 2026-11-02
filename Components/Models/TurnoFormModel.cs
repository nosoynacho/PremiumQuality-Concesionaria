using System.ComponentModel.DataAnnotations;

namespace blazornacho.Models
{
    public class TurnoFormModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Seleccione una fecha.")]
        [DataType(DataType.Date)]
        
        public DateTime? Fecha { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Seleccione un horario.")]
        public string Hora { get; set; } = "";
        
        //Esto indica que es obligatorio completar el campo.
        [Required(ErrorMessage = "Ingrese el nombre completo.")]
        [StringLength(
            80,
            MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 80 caracteres."
        )]
        //Eso valida el formato del nombre, permitiendo solo letras y espacios.
        [RegularExpression(
            @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]+$",
            ErrorMessage = "El nombre solo puede contener letras y espacios."
        )]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "Ingrese el motivo del turno.")]
        [StringLength(
            250,
            MinimumLength = 5,
            ErrorMessage = "El motivo debe tener entre 5 y 250 caracteres."
        )]
        public string Motivo { get; set; } = "";
    }
}