using System;
using System.ComponentModel.DataAnnotations;
using GestionDeInventario.Models;

namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionCreateDTO
    {
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
        public string? Motivo { get; set; }
    }
}
