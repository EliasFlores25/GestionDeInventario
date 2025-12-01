using System;
using System.Collections.Generic;
using System.Linq;
using GestionDeInventario.Data;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Repository.Implementations
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly AppDbContext _context;

        public DepartamentoRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Departamento> GetQueryable()
        {
            return _context.Departamentos.AsNoTracking();
        }
        public async Task<List<Departamento>> GetAllAsync()
         => await _context.Departamentos.AsNoTracking().ToListAsync();
        public async Task<Departamento?> GetByIdAsync(int idDepartamento)
            => await _context.Departamentos.FindAsync(idDepartamento);
        public async Task<Departamento> AddAsync(Departamento entity)
        {
            _context.Departamentos.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> UpdateAsync(Departamento entity)
        {
            _context.Departamentos .Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteAsync(int idDepartamento)
        {
            var existing = await _context.Departamentos.FindAsync(idDepartamento);
            if (existing == null) return false;
            _context.Departamentos.Remove(existing);
            return await _context.SaveChangesAsync() > 0;
        }
      
    }
}