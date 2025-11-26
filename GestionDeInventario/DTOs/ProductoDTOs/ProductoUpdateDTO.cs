namespace GestionDeInventario.DTOs.ProductoDTOs
{
    public class ProductoUpdateDTO
    {
       public int idProducto { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int cantidadStock { get; set; }
        public string unidadMedida { get; set; }
        public decimal precio { get; set; }
        public string estado { get; set; }
    }
}
