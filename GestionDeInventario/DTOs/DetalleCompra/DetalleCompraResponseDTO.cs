using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleCompra
{
    public class DetalleCompraResponseDTO
    {
        public int IdDetalleCompra { get; set; }
        public string NumeroFactura { get; set; }
        public int UsuarioId { get; set; }
        public int ProveedorId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitarioCosto { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaCompra { get; set; }
    }
}
