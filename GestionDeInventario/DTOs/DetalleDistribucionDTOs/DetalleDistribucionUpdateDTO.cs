namespace GestionDeInventario.DTOs.DetalleDistribucionDTOs
{
    public class DetalleDistribucionUpdateDTO
    {
        public int IdDetalleDistribucion { get; set; }
        public string NumeroDistribucion { get; set; }
        public int UsuarioId { get; set; }
        public int EmpleadoId   { get; set; }
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }
        public DateTime FechaSalida { get; set; }
        public string Motivo { get; set; }
        public decimal PrecioCostoUnitario { get; set; }
        public decimal MontoTotal { get; set; }
    }
}
