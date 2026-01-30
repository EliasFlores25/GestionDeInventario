using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface ICompraRepository
    {
        IQueryable<Compra> GetQueryable();
        Task<List<Compra>> GetAllAsync();
        Task<Compra?> GetByIdAsync(int idCompra);
        Task<Compra> AddAsync(Compra entity);
        Task<bool> UpdateAsync(Compra entity);
        Task<bool> DeleteAsync(int idCompra);
    }
}
