using GestionDeInventario.Data;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Repository.Implementations
{
    public class DetalleDistribucionRepository: IDetalleDistribucionRepository

    {
        private readonly AppDbContext _context;
        public DetalleDistribucionRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<DetalleDistribucion> GetQueryable() =>
            _context.DetalleDistribuciones.AsNoTracking();
        public async Task<List<DetalleDistribucion>> GetAllAsync() =>
            await _context.DetalleDistribuciones.AsNoTracking().ToListAsync();
        public async Task<DetalleDistribucion?> GetByIdAsync(int idDetalleDistribucion)
             => await _context.DetalleDistribuciones.AsNoTracking()
            .Include(e => e.Usuario)
            .Include(e => e.Producto)
            .Include(e => e.Empleado)
            .FirstOrDefaultAsync(e => e.IdDetalleDistribucion == idDetalleDistribucion);
        public async Task<DetalleDistribucion> AddAsync(DetalleDistribucion entity)
        {
            _context.DetalleDistribuciones.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(DetalleDistribucion entity)
        {
            _context.DetalleDistribuciones.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(int idDetalleDistribucion)
        {
            var existing = await _context.DetalleDistribuciones.FindAsync(idDetalleDistribucion);
            if (existing == null) return false;
            _context.DetalleDistribuciones.Remove(existing);
            return await _context.SaveChangesAsync() >= 0;
        }


    }
}
