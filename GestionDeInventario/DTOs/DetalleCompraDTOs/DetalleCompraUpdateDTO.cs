using System;
using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraUpdateDTO
    {
        [Required(ErrorMessage = "El id del detalle de compra es obligatorio")]
        public int idDetalleCompra { get; set; }

        [Required(ErrorMessage = "El número de factura es obligatorio")]
        [StringLength(50, ErrorMessage = "El número de factura no puede exceder 50 caracteres")]
        public string numeroFactura { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int usuarioId { get; set; }

        [Required(ErrorMessage = "El proveedor es obligatorio")]
        public int proveedorId { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio")]
        public int productoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor a 0")]
        public decimal precioUnitarioCosto { get; set; }

        [Required(ErrorMessage = "El monto total es obligatorio")]
        public decimal montoTotal { get; set; }

        [Required(ErrorMessage = "La fecha de compra es obligatoria")]
        public DateTime fechaCompra { get; set; }
    }
}
