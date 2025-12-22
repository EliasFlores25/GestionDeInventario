
namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraCreateDTO
    {
        public string numeroFactura { get; set; }

        public int usuarioId { get; set; }

        public int proveedorId { get; set; }

        public int productoId { get; set; }

        public int cantidad { get; set; }

        public decimal precioUnitarioCosto { get; set; }

        public decimal montoTotal { get; private set; }

        public DateTime fechaCompra { get; set; }
    }
}
