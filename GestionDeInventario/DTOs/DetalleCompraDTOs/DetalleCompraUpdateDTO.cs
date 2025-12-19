using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraUpdateDTO
    {
        public int idDetalleCompra { get; set; }
        public string numeroFactura { get; set; }
        public int usuarioId { get; set; }
        public int proveedorId { get; set; }
        public int productoId { get; set; }
        public int cantidad { get; set; }
        public decimal precioUnitarioCosto { get; set; }
        public decimal montoTotal { get; set; }
        public DateTime fechaCompra { get; set; }
    }
}
