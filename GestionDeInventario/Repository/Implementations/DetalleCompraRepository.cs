using GestionDeInventario.Data;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Repository.Implementations
{
    public class DetalleCompraRepository : IDetalleCompraRepository
    {
        private readonly AppDbContext _context;

        public DetalleCompraRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<DetalleCompra> GetQueryable() =>
            _context.DetalleCompras.AsNoTracking();
        public async Task<List<DetalleCompra>> GetAllAsync() =>
            await _context.DetalleCompras.AsNoTracking().ToListAsync();
        public async Task<DetalleCompra?> GetByIdAsync(int idDetalleCompra)
             => await _context.DetalleCompras.AsNoTracking()
            .Include(e => e.usuario)
            .Include(e => e.producto)
            .Include(e => e.proveedor)
            .FirstOrDefaultAsync(e => e.idDetalleCompra == idDetalleCompra);
        public async Task<DetalleCompra> AddAsync(DetalleCompra entity)
        {
            _context.DetalleCompras.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(DetalleCompra entity)
        {
            _context.DetalleCompras.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(int idDetalleCompra)
        {
            var existing = await _context.DetalleCompras.FindAsync(idDetalleCompra);
            if (existing == null) return false;
            _context.DetalleCompras.Remove(existing);
            return await _context.SaveChangesAsync() >= 0;
        }
    }
}
