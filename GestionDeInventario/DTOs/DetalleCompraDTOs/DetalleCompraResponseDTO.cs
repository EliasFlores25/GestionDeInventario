namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraResponseDTO
    {
        public int idDetalleCompra { get; set; }
        public string numeroFactura { get; set; } = null!;
        public string productoNombre { get; set; } = null!;
        public int cantidad { get; set; }
        public decimal precioUnitarioCosto { get; set; }
        public decimal montoTotal { get; set; }
        public DateTime fechaCompra { get; set; }
    }
}
