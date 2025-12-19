using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    [Table("Producto")]
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int cantidadStock { get; set; }
        public string unidadMedida { get; set; }
        public decimal precio { get; set; }
        public string estado { get; set; }
        public ICollection<DetalleCompra> detalleCompras { get; set; } = new List<DetalleCompra>();
    }
}
