using GestionDeInventario.Models;

namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraResponseDTO
    {
        public int IdDetalleCompra { get; set; }
        public int CompraId { get; set; }
        public Compra Compra { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitarioCosto { get; set; }
        public decimal Subtotal { get; set; }
    }
}
