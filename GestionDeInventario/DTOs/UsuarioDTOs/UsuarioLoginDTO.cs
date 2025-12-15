using System.ComponentModel.DataAnnotations;

namespace GestionDeInventario.DTOs.UsuarioDTOs
{
    public class UsuarioLoginDTO
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        public string email { get; set; }
        [Required(ErrorMessage = "El tipo de rol es obligatorio.")]
        public string contraseña { get; set; }
    }
}