using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IDistribucionRepository
    {
        IQueryable<Distribucion> GetQueryable();
        Task<List<Distribucion>> GetAllAsync();
        Task<Distribucion> AddAsync(Distribucion entity);
        Task<Distribucion?> GetByIdAsync(int idDistribucion);
        Task<bool> UpdateAsync(Distribucion entity);
        Task<bool> DeleteAsync(int idDistribucion);
    }
}
