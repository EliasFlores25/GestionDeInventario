using GestionDeInventario.Data;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Repository.Implementations
{
    public class CompraRepository : ICompraRepository
    {
        public readonly AppDbContext _context;
        public CompraRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Compra> GetQueryable()
        => _context.Compras.AsNoTracking();

        public async Task<List<Compra>> GetAllAsync()
        => await _context.Compras.AsNoTracking().ToListAsync();

        public async Task<Compra?> GetByIdAsync(int idCompra)
        => await _context.Compras.AsNoTracking().
            Include(c => c.Usuario).
            Include(c => c.Proveedor).
            FirstOrDefaultAsync(c => c.IdCompra == idCompra);

        public async Task<Compra> AddAsync(Compra entity)
        {
            _context.Compras.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(Compra entity)
        {
            _context.Compras.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(int idCompra)
        {
            var existing = await _context.Compras.FindAsync(idCompra);
            if (existing == null) return false;
            _context.Compras.Remove(existing);
            return await _context.SaveChangesAsync() >= 0;

        }
    }
}
