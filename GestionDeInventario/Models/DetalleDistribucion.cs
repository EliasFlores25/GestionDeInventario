using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    [Table("DetalleDistribucion")]
    public class DetalleDistribucion
    {
        [Key]
        public int IdDetalleDistribucion { get; set; }
        public string NumeroDistribucion { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public int UsuarioId { get; set; }
        public Empleado Empleado { get; set; } = null!;
        public int EmpleadoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaSalida { get; set; }
        public string? Motivo { get; set; }
        public decimal PrecioCostoUnitario { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal MontoTotal { get; set; }
    }
}
