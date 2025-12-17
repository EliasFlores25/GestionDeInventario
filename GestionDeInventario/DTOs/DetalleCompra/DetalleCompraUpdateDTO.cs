using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraUpdateDTO
    {
        [Required]
        public int IdDetalleCompra { get; set; }

        [Required]
        public string NumeroFactura { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ProveedorId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitarioCosto { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; }
    }
}
