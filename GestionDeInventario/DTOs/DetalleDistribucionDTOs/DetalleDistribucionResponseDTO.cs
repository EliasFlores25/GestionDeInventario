using GestionDeInventario.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionResponseDTO
    {
        public int IdDetalleDistribucion { get; set; }
        public int DistribucionId { get; set; }
        public Distribucion Distribucion { get; set; } = null!;
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioCostoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
