using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IProductoRepository
    {
        IQueryable<Producto> GetQueryable();
        Task<List<Producto>> GetAllAsync();
        Task<Producto?> GetByIdAsync(int idProducto);
        Task<Producto> AddAsync(Producto entity);
        Task<bool> UpdateAsync(Producto entity);
        Task<bool> DeleteAsync(int idProducto);
    }
}
