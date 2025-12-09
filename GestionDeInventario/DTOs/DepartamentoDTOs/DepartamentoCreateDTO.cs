using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DepartamentoDTOs
{
    public class DepartamentoCreateDTO
    {
        [Required(ErrorMessage = "El nombre del departamento es obligatorio.")]
        [StringLength(100)]
        public string nombre { get; set; }
        [Required(ErrorMessage = "La descripción del departamento es obligatoria.")]
        [StringLength(250)]
        public string descripcion { get; set; }
    }
}
