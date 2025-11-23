using GestionDeInventario.Data;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Repository.Implementations
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly AppDbContext _context;
        public EmpleadoRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Empleado> GetQueryable()
        {
            return _context.Empleados.AsNoTracking();
        }
        public async Task<List<Empleado>> GetAllAsync()
          => await _context.Empleados.AsNoTracking().ToListAsync();
        public async Task<Empleado?> GetByIdAsync(int id)
            => await _context.Empleados.FindAsync(id);
        public async Task<Empleado> AddAsync(Empleado entity)
        {
            _context.Empleados.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(Empleado entity)
        {
            _context.Empleados.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Empleados.FindAsync(id);
            if (existing == null) return false;
            _context.Empleados.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
