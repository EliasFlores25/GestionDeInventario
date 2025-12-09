using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionDeInventario.Models
{
    [Table("Departamento")]
    public class Departamento
    {
        [Key]
        public int idDepartamento { get; set; }
        public string nombre { get; set; }
        public string? descripcion { get; set; }
        public ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    }
}

