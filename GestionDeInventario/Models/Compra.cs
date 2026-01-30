using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    [Table("Compra")]
    public class Compra
    {
        [Key]
        public int IdCompra { get; set; }
        public string NumeroFactura { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; } = null!;
        public DateTime FechaCompra { get; set; }
        public decimal MontoTotalCompra { get; set; } 
        public  ICollection<DetalleCompra> DetalleCompras { get; set; } =new List<DetalleCompra>(); 

    }
}
