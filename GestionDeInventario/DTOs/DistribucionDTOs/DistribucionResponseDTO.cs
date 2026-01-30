using GestionDeInventario.Models;

namespace GestionDeInventario.DTOs.DistribucionDTOs
{
    public class DistribucionResponseDTO
    {
        public int IdDistribucion { get; set; }
        public string NumeroDistribucion { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
        public DateTime FechaSalida { get; set; }
        public string? Motivo { get; set; }
        public decimal MontoTotalDistribucion { get; set; }
    }
}
