using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IProveedorRepository
    {
        IQueryable<Proveedor> GetQueryable();
        Task<List<Proveedor>> GetAllAsync();
        Task<Proveedor?> GetByIdAsync(int idProveedor);
        Task<Proveedor> AddAsync(Proveedor entity);
        Task<bool> UpdateAsync(Proveedor entity);
        Task<bool> DeleteAsync(int idProveedor);
    }
}
