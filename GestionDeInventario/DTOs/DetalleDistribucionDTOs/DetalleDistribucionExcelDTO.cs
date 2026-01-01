using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionExcelDTO
    {
        public int IdDetalleDistribucion { get; set; }

        [Display(Name = "Número de Distribución")]
        public string NumeroDistribucion { get; set; }

        [Display(Name = "Fecha de Salida")]
        public DateTime FechaSalida { get; set; }

        [Display(Name = "Empleado")]
        public string NombreEmpleado { get; set; }

        [Display(Name = "Producto")]
        public string NombreProducto { get; set; }

        public int Cantidad { get; set; }

        public string Motivo { get; set; }

        [Display(Name = "Precio Costo Unitario")]
        public decimal PrecioCostoUnitario { get; set; }

        [Display(Name = "Valor Total")]
        public decimal MontoTotal { get; set; }

        [Display(Name = "Usuario Registro")]
        public string UsuarioRegistro { get; set; }
    }
}
