using System;
using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionUpdateDTO
    {
        [Required(ErrorMessage = "El id del detalle de distribución es obligatorio")]
        public int IdDetalleDistribucion { get; set; }

        [Required(ErrorMessage = "El número de distribución es obligatorio")]
        [StringLength(50, ErrorMessage = "El número de distribución no puede exceder 50 caracteres")]
        public string NumeroDistribucion { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El empleado es obligatorio")]
        public int EmpleadoId { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "La fecha de salida es obligatoria")]
        public DateTime FechaSalida { get; set; }

        [StringLength(200, ErrorMessage = "El motivo no puede exceder 200 caracteres")]
        public string Motivo { get; set; }

        [Required(ErrorMessage = "El precio costo unitario es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PrecioCostoUnitario { get; set; }

        [Required(ErrorMessage = "El monto total es obligatorio")]
        public decimal MontoTotal { get; set; }
    }
}
