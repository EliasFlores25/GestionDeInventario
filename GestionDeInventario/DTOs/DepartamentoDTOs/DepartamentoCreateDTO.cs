using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DepartamentoDTOs
{
    public class DepartamentoCreateDTO
    {
        [Required(ErrorMessage = "El nombre del departamento es obligatorio.")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "La descripción del departamento es obligatoria.")]
        public string descripcion { get; set; }
    }
}
