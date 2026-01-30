using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    [Table("Distribucion")]
    public class Distribucion
    {
        [Key]
        public int IdDistribucion { get; set; }
        public string NumeroDistribucion { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; } = null!;
        public DateTime FechaSalida { get; set; }
        public string? Motivo { get; set; }
        public decimal MontoTotalDistribucion { get; set; }
        public ICollection<DetalleDistribucion> DetalleDistribuciones { get; set; } = new List<DetalleDistribucion>();
    }
}
