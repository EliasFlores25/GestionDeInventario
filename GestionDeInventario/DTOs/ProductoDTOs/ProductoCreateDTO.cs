using System.ComponentModel.DataAnnotations;
namespace GestionDeInventario.DTOs.ProductoDTOs
   
{
    public class ProductoCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string descripcion { get; set; }
        [Required(ErrorMessage = "La cantidad en stock es obligatoria.")]
        public int cantidadStock { get; set; }
        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        public string unidadMedida { get; set; }
        [Required(ErrorMessage = "El precio es obligatorio.")]
        public decimal precio { get; set; }
        [Required(ErrorMessage = "El estado es obligatorio.")]
        public string estado { get; set; }
    }
}
