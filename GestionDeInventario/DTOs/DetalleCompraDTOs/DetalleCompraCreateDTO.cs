using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraCreateDTO
    {
        [Required]
        public string numeroFactura { get; set; } = null!;

        [Required]
        public int productoId { get; set; }

        [Required]
        public int cantidad { get; set; }

        [Required]
        public decimal precioUnitarioCosto { get; set; }

        [Required]
        public DateTime fechaCompra { get; set; }

        [Required]
        public int usuarioId { get; set; }

        [Required]
        public int proveedorId { get; set; }
    }
}
