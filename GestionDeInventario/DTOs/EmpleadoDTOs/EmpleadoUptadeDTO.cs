using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.EmpleadoDTOs
{
    public class EmpleadoUptadeDTO
    {
        public int idEmpleado { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string apellido { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(18, 100, ErrorMessage = "La edad debe estar entre 18 y 100 años.")]
        public int edad { get; set; }

        [Required(ErrorMessage = "El género es obligatorio.")]
        public string genero { get; set; }

        // Teléfono y Expresión Regular
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe contener solo números de 8 dígitos.")]
        public string telefono { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public string direccion { get; set; }
        public int departamentoId { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public string estado { get; set; }
    }
}
