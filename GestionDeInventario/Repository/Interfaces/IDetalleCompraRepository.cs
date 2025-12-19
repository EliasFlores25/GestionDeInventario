using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IDetalleCompraRepository
    {
        IQueryable<DetalleCompra> GetQueryable();
        Task<List<DetalleCompra>> GetAllAsync();
        Task<DetalleCompra?> GetByIdAsync(int idDetalleCompra);
        Task<DetalleCompra> AddAsync(DetalleCompra entity);
        Task<bool> UpdateAsync(DetalleCompra entity);
        Task<bool> DeleteAsync(int idDetalleCompra);
    }
}
