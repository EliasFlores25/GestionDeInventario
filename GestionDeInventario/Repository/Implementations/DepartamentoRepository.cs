using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Departamento>> GetAll()
        {
            return await _context.Departamento
                                 .AsNoTracking()
                                 .OrderBy(d => d.nombre)
                                 .ToListAsync();
        }

        public async Task<Departamento> GetById(int id)
        {
            var departamento = await _context.Departamento
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(d => d.idDepartamento == id);

            if (departamento == null)
                throw new KeyNotFoundException($"Departamento con id {id} no encontrado.");

            return departamento;
        }

        public async Task Add(Departamento departamento)
        {
            if (departamento is null)
                throw new ArgumentNullException(nameof(departamento));

            _context.Departamento.Add(departamento);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Departamento departamento)
        {
            if (departamento is null)
                throw new ArgumentNullException(nameof(departamento));

            var exists = await _context.Departamento
                                       .AnyAsync(d => d.idDepartamento == departamento.idDepartamento);

            if (!exists)
                throw new KeyNotFoundException($"No existe un departamento con id {departamento.idDepartamento} para actualizar.");

            _context.Entry(departamento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Error de concurrencia al actualizar el departamento.", ex);
            }
        }

        public async Task Delete(int id)
        {
            var departamento = await _context.Departamento.FindAsync(id);

            if (departamento == null)
                throw new KeyNotFoundException($"Departamento con id {id} no encontrado para eliminar.");

            _context.Departamento.Remove(departamento);
            await _context.SaveChangesAsync();
        }
    }
}