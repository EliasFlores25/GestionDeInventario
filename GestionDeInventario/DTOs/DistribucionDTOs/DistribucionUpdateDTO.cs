using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DistribucionDTOs
{
    public class DistribucionUpdateDTO
    {
        public int IdDistribucion { get; set; }
        [Required(ErrorMessage = "El número de distribución es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número de distribución no debe exceder los 50 caracteres.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        public string NumeroDistribucion { get; set; }
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int UsuarioId { get; set; }
        [Required(ErrorMessage = "El empleado es obligatorio.")]
        public int EmpleadoId { get; set; }
        [Required(ErrorMessage = "La fecha de salida es obligatorio.")]
        public DateTime FechaSalida { get; set; }
        public string? Motivo { get; set; }
    }
}
