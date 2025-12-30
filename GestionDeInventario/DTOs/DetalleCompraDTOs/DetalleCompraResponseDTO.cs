using System;
using System.ComponentModel.DataAnnotations;
using GestionDeInventario.Models;

namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraResponseDTO
    {
        [Required]
        public int idDetalleCompra { get; set; }

        [Required]
        [StringLength(50)]
        public string numeroFactura { get; set; }

        [Required]
        public int usuarioId { get; set; }

        public Usuario usuario { get; set; }

        [Required]
        public int proveedorId { get; set; }

        public Proveedor proveedor { get; set; }

        [Required]
        public int productoId { get; set; }

        public Producto producto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int cantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal precioUnitarioCosto { get; set; }

        [Required]
        public decimal montoTotal { get; set; }

        [Required]
        public DateTime fechaCompra { get; set; }
    }
}
