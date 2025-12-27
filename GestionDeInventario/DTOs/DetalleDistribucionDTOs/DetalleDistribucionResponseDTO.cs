namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionResponseDTO
    {
        public int IdDetalleDistribucion { get; set; }
        public string NumeroDistribucion { get; set; }

        public int IdUsuario { get; set; }
        public int IdEmpleado { get; set; }
        public int IdProducto { get; set; }

        public int Cantidad { get; set; }
        public DateTime FechaSalida { get; set; }
        public string Motivo { get; set; }

        public decimal PrecioCostoUnitario { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
