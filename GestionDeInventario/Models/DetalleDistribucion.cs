using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    [Table("DetalleDistribucion")]
    public class DetalleDistribucion
    {
        [Key]
        public int IdDetalleDistribucion { get; set; }
        public int DistribucionId { get; set; }
        public Distribucion Distribucion { get; set; } = null!;
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioCostoUnitario { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal Subtotal { get; set; }
    }
}
