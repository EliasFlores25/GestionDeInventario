using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GestionDeInventario.Models
{
    [Table("Usuario")]
    public class Usuario
    {
     [Key]
        public int idUsuario { get; set; }
        public string nombre { get; set; }
        public string tipoRol { get; set; }
        public string email { get; set; }
        public string contraseña{ get; set; }
        public ICollection<Compra> DetallesCompras { get; set; } = new List<Compra>();
        public ICollection<Distribucion> DetallesDistribuciones { get; set; } = new List<Distribucion>();
    }
}