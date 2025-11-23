using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    [Table("Empleado")]
    public class Empleado
    {
        [Key]
        public int idEmpleado { get; set; }
        public string? nombre { get; set; }
        public string? apellido { get; set; }
        public int edad { get; set; }
        public string? genero { get; set; }
        public string? telefono { get; set; }
        public string? direccion { get; set; }
        public int departamentoId { get; set; }
        public string? estado { get; set; }
    }
}
