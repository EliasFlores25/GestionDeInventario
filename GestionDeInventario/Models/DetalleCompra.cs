using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    public class DetalleCompra
    {
        [Key]
        public int IdDetalleCompra { get; set; }

        [Required]
        public string NumeroFactura { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ProveedorId { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitarioCosto { get; set; }

        // Campo calculado en la base de datos
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal MontoTotal { get; private set; }

        [Required]
        public DateTime FechaCompra { get; set; }
    }
}
