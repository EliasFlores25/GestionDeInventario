using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.EmpleadoDTOs
{
    public class EmpleadoCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string apellido { get; set; }

        [Range(18, 100, ErrorMessage = "La edad debe estar entre 18 y 100 años.")]
        [Required(ErrorMessage = "La edad es obligatoria.")]
        public int edad { get; set; }

        [Required(ErrorMessage = "El género es obligatorio.")]
        public string genero { get; set; }

        // Teléfono y Expresión Regular
       
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe contener solo números de 8 dígitos.")]
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(8, MinimumLength = 1)]
        public string telefono { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "La dirección debe tener entre 10 y 255 caracteres.")]
        public string direccion { get; set; }
        public int departamentoId { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public string estado { get; set; }
    }
}
