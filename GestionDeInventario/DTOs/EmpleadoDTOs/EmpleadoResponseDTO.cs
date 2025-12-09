using GestionDeInventario.Models;
using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.EmpleadoDTOs
{
    public class EmpleadoResponseDTO
    {
        public int idEmpleado { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public int edad { get; set; }
        public string genero { get; set; }
        public string telefono { get; set; }
        public string direccion { get; set; }
        public Departamento departamento { get; set; }
        public int departamentoId { get; set; }
        public string estado { get; set; }
    }
}
