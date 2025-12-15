using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        IQueryable<Usuario> GetQueryable();
        Task<Usuario?> GetByEmailAsync(string email); //LoginAsync
        Task<Usuario?> GetByIdAsync(int idUsuario);
        Task<Usuario> AddAsync(Usuario entity); //RegistrarAsync
        Task<List<Usuario>> GetAllAsync();
        Task<bool> UpdateAsync(Usuario entity);
        Task<bool> DeleteAsync(int idUsuario);
    }
}