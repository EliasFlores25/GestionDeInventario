using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.CompraDTOs
{
    public class CompraUpdateDTO
    {
        public int IdCompra { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres.")]
        [MinLength(2, ErrorMessage = "El nombre debe tener al menos 2 caracteres.")]
        public string NumeroFactura { get; set; }
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int UsuarioId { get; set; }
        [Required(ErrorMessage = "El proveedor es obligatorio.")]
        public int ProveedorId { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public DateTime FechaCompra { get; set; }
    }
}
