using System;
using System.ComponentModel.DataAnnotations;
using GestionDeInventario.Models;

namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionCreateDTO
    {
        [Required(ErrorMessage = "La distribución es obligatorio")]
        public int DistribucionId { get; set; }


        [Required(ErrorMessage = "El producto es obligatorio")]
        public int ProductoId { get; set; }


        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }
    }
}
