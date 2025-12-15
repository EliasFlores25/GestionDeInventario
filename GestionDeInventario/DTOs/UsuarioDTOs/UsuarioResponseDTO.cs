using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.UsuarioDTOs
{
    public class UsuarioResponseDTO
    {
        public int idUsuario { get; set; }
        public string nombre { get; set; }
        public string tipoRol { get; set; }
        public string email { get; set; }
        public string contraseña { get; set; }
    }
}