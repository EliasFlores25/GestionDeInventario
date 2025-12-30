using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IDetalleDistribucionRepository
    {
        IQueryable<DetalleDistribucion> GetQueryable();
        Task<List<DetalleDistribucion>> GetAllAsync();
        Task<DetalleDistribucion?> GetByIdAsync(int idDetalleDistribucion);
        Task<DetalleDistribucion> AddAsync(DetalleDistribucion entity);
        Task<bool> UpdateAsync(DetalleDistribucion entity);
        Task<bool> DeleteAsync(int idDetalleDistribucion);
    }
}
