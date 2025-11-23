using GestionDeInventario.Models;

namespace GestionDeInventario.Services.Interfaces
{
    public interface IEmpleadoService
    {
        IQueryable<Empleado> GetQueryable();
        Task<List<Empleado>> GetAllAsync();
        Task<Empleado?> GetByIdAsync(int id);
        Task<Empleado> AddAsync(Empleado categoria);
        Task<bool> UpdateAsync(int id, Empleado categoria);
        Task<bool> DeleteAsync(int id);
    }
}
