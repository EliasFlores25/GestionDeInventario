using GestionDeInventario.Models;

namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionCreateDTO
    {
        public string NumeroDistribucion { get; set; }
        public int UsuarioId { get; set; }
        public int EmpleadoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaSalida { get; set; }
        public string? Motivo { get; set; }
    }
}
