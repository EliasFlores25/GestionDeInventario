using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DepartamentoDTOs
{
    public class DepartamentoCreateDTO
    {
        [Required(ErrorMessage = "El nombre del departamento es obligatorio.")]
        public string nombre { get; set; }

        public string? descripcion { get; set; }
    }
}
