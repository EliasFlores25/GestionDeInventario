using GestionDeInventario.Data;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Repository.Implementations
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly AppDbContext _context;
        public ProveedorRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Proveedor> GetQueryable()=>
            _context.Proveedores.AsNoTracking();
        public async Task<List<Proveedor>> GetAllAsync()
       => await _context.Proveedores.AsNoTracking().ToListAsync();
        public async Task<Proveedor?> GetByIdAsync(int idProveedor)
           => await _context.Proveedores.FindAsync(idProveedor);
        public async Task<Proveedor> AddAsync(Proveedor entity)
        {
            _context.Proveedores.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(Proveedor entity)
        {
            _context.Proveedores.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(int idProveedor)
        {
            var existing = await _context.Proveedores.FindAsync(idProveedor);
            if (existing == null) return false;
            _context.Proveedores.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}