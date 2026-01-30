using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraUpdateDTO
    {
        public int IdDetalleCompra { get; set; }
        [Required(ErrorMessage = "La compra es obligatoria")]
        public int CompraId { get; set; }
        [Required(ErrorMessage = "El producto es obligatorio")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        public int Cantidad { get; set; }
        [Required(ErrorMessage = "El precio unitario es obligatoria")]
        public decimal PrecioUnitarioCosto { get; set; }
    }
}
