using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionExcelDTO
    {
        public int IdDetalleDistribucion { get; set; }
        [Display(Name = "Distribucion")]
        public string NumeroDistribucion { get; set; }
        [Display(Name = "Producto")]
        public string nombre { get; set; }
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }
        [Display(Name = "Precio Unitario")]
        public decimal PrecioCostoUnitario { get; set; }
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }
    }
}
