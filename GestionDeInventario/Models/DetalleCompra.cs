using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    [Table("DetalleCompra")]
    public class DetalleCompra
    {
        [Key]
        public int idDetalleCompra { get; set; }
        public string numeroFactura { get; set; }
        public int cantidad { get; set; }
        public decimal precioUnitarioCosto { get; set; }
        public decimal montoTotal { get; set; } 
        public DateTime fechaCompra { get; set; }
        public int usuarioId { get; set; }
        public Usuario usuario { get; set; } = null!;
        public int productoId { get; set; }
        public Producto producto { get; set; } = null!;
        public Proveedor proveedor { get; set; } = null!;
        public int proveedorId { get; set; }
    }
}