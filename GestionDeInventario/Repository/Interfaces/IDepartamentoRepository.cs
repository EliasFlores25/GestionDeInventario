using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IDepartamentoRepository
    {
        IQueryable<Departamento> GetQueryable();
        Task<List<Departamento>> GetAllAsync();
        Task<Departamento?> GetByIdAsync(int idDepartamento);
        Task<Departamento> AddAsync(Departamento entity);
        Task<bool> UpdateAsync(Departamento entity);
        Task<bool> DeleteAsync(int idDepartamento);
    }
}