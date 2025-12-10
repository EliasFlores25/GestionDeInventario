using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DepartamentoDTOs
{
    public class DepartamentoUpdateDTO
    {
        public int idDepartamento { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "El nombre debe tener entre 10 y 255 caracteres.")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 255 caracteres.")]
        public string descripcion { get; set; }
    }
}

