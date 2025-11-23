using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IEmpleadoRepository
    {
        IQueryable<Empleado> GetQueryable();
        Task<List<Empleado>> GetAllAsync();
        Task<Empleado?> GetByIdAsync(int id);
        Task<Empleado> AddAsync(Empleado entity);
        Task<bool> UpdateAsync(Empleado entity);
        Task<bool> DeleteAsync(int id);
    }
}
