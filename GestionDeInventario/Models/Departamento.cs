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

        [Required]
        [StringLength(100)]
        public string nombre { get; set; }

        [StringLength(250)]
        public string? descripcion { get; set; }
    }
}

