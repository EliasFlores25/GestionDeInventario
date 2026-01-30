using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraExcelDTO
    {
        public int IdDetalleCompra { get; set; }
        [Display(Name = "Compra")]
        public string NumeroFactura { get; set; }
        [Display(Name = "Producto")]
        public string nombre { get; set; }
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }
        [Display(Name = "Precio Unitariio")]
        public decimal PrecioUnitarioCosto { get; set; }
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }
    }
}
