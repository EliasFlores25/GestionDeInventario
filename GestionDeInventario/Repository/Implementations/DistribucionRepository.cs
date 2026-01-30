using GestionDeInventario.Data;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Repository.Implementations
{
    public class DistribucionRepository : IDistribucionRepository
    {
        private readonly AppDbContext _context;
        public DistribucionRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Distribucion> GetQueryable()
            => _context.Distribuciones.AsNoTracking();
        public async Task<List<Distribucion>> GetAllAsync()
       => await _context.Distribuciones.AsNoTracking().ToListAsync();
        public async Task<Distribucion?> GetByIdAsync(int idDistribucion)
        => await _context.Distribuciones.AsNoTracking()
            .Include(d => d.Usuario)
            .Include(d => d.Empleado)
            .FirstOrDefaultAsync(d => d.IdDistribucion==idDistribucion);
        public async Task<Distribucion> AddAsync(Distribucion entity)
        {
            _context.Distribuciones.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(Distribucion entity)
        {
            _context.Distribuciones.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(int idDistribucion)
        {
            var existing = await _context.Distribuciones.FindAsync(idDistribucion);
            if (existing == null) return false;
            _context.Distribuciones.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
