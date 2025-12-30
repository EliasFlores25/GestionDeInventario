using System;
using System.ComponentModel.DataAnnotations;
using GestionDeInventario.Models;

namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionResponseDTO
    {
        [Required]
        public int IdDetalleDistribucion { get; set; }

        [Required]
        [StringLength(50)]
        public string NumeroDistribucion { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        [Required]
        public int EmpleadoId { get; set; }

        public Empleado Empleado { get; set; }

        [Required]
        public int ProductoId { get; set; }

        public Producto Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Required]
        public DateTime FechaSalida { get; set; }

        [StringLength(200)]
        public string Motivo { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PrecioCostoUnitario { get; set; }

        [Required]
        public decimal MontoTotal { get; set; }
    }
}
