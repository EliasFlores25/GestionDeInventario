using GestionDeInventario.Models;

namespace GestionDeInventario.DTOs.CompraDTOs
{
    public class CompraResponseDTO
    {
        public int IdCompra { get; set; }
        public string NumeroFactura { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; } 
        public DateTime FechaCompra { get; set; }
        public decimal MontoTotalCompra { get; set; }

    }
}
