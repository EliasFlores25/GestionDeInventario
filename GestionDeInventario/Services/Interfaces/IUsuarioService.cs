using GestionDeInventario.DTOs.UsuarioDTOs;
namespace GestionDeInventario.Services.Interfaces
{
    public interface IUsuarioService
    {
        IQueryable<UsuarioResponseDTO> GetQueryable();
        Task<UsuarioResponseDTO> GetByEmailAsync(string email);
        Task<UsuarioResponseDTO> GetByIdAsync(int idUsuario);
        Task<UsuarioResponseDTO> AddAsync(UsuarioRegisterDTO dto);
        Task<List<UsuarioResponseDTO>> GetAllAsync();
        Task<bool> UpdateAsync(int idUsuario, UsuarioUpdateDTO dto);
        Task<bool> DeleteAsync(int idUsuario);
    }
}