using GestionDeInventario.Data;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Repository.Implementations
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;
        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Producto> GetQueryable()
        {
            return _context.Productos.AsNoTracking();
        }
        public async Task<List<Producto>> GetAllAsync()
          => await _context.Productos.AsNoTracking().ToListAsync();
        public async Task<Producto?> GetByIdAsync(int idProducto)
            => await _context.Productos.FindAsync(idProducto);
        public async Task<Producto> AddAsync(Producto entity)
        {
            _context.Productos.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(Producto entity)
        {
            _context.Productos.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(int idProducto)
        {
            var existing = await _context.Productos.FindAsync(idProducto);
            if (existing == null) return false;
            _context.Productos.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
