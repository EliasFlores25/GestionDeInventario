using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleCompraDTOs
{
    public class DetalleCompraExcelDTO
    {
        public int IdDetalleCompra { get; set; }

        [Display(Name = "Número de Factura")]
        public string NumeroFactura { get; set; }

        [Display(Name = "Fecha de Compra")]
        public DateTime FechaCompra { get; set; }

        [Display(Name = "Proveedor")]
        public string NombreProveedor { get; set; }

        [Display(Name = "Producto")]
        public string NombreProducto { get; set; }

        public int Cantidad { get; set; }

        [Display(Name = "Precio Unitario")]
        public decimal PrecioUnitarioCosto { get; set; }

        [Display(Name = "Monto Total")]
        public decimal MontoTotal { get; set; }

        [Display(Name = "Usuario Registro")]
        public string UsuarioRegistro { get; set; }
    }
}
