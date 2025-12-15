using GestionDeInventario.DTOs.UsuarioDTOs;

namespace GestionDeInventario.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UsuarioResponseDTO> RegistrarAsync(UsuarioRegisterDTO dto);
        Task<UsuarioResponseDTO?> LoginAsync(UsuarioLoginDTO dto);
    }
}
