using GestionDeInventario.Models;

namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionResponseDTO
    {
        public int IdDetalleDistribucion { get; set; }
        public string NumeroDistribucion { get; set; }
        public Usuario Usuario { get; set; }
        public int UsuarioId { get; set; }
        public Empleado Empleado { get; set; }
        public int EmpleadoId { get; set; }
        public Producto Producto { get; set; }
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }
        public DateTime FechaSalida { get; set; }
        public string Motivo { get; set; }

        public decimal PrecioCostoUnitario { get; set; }
        public decimal MontoTotal { get; set; }

    }
}
