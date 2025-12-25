using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    [Table("Proveedor")]
    public class Proveedor
    {
        [Key]
        public int idProveedor { get; set; }
        public string nombreEmpresa { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public string estado { get; set; }
        public ICollection<DetalleCompra> detalleCompras  { get; set; }=new List<DetalleCompra> ();
    }
}
