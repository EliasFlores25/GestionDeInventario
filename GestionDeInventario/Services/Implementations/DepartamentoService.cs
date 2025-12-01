using GestionDeInventario.DTOs.DepartamentoDTOs;
using GestionDeInventario.DTOs.ProductoDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IDepartamentoRepository _repo;
        public DepartamentoService(IDepartamentoRepository repo)
        {
            _repo = repo;
        }
        private DepartamentoResponseDTO MapToResponseDTO(Departamento x)
        {
            return new DepartamentoResponseDTO
            {
                idDepartamento = x.idDepartamento,
                nombre = x.nombre,
                descripcion = x.descripcion,
            };
        }

        public IQueryable<DepartamentoResponseDTO> GetQueryable()
        {
            return _repo.GetQueryable().Select(x => new DepartamentoResponseDTO
            {
                idDepartamento = x.idDepartamento,
                nombre = x.nombre,
                descripcion = x.descripcion,
            });
        }
        public async Task<List<DepartamentoResponseDTO>> GetAllAsync() =>
           (await _repo.GetAllAsync()).Select(MapToResponseDTO).ToList();

        public async Task<DepartamentoResponseDTO> GetByIdAsync(int idDepartamento)
        {
            var x = await _repo.GetByIdAsync(idDepartamento);
            if (x == null)
            {
                throw new NotFoundException($"Departamento con ID {idDepartamento} no encontrado.");
            }
            return MapToResponseDTO(x);
        }

        public async Task<DepartamentoResponseDTO> AddAsync(DepartamentoCreateDTO dto)
        {
            var entity = new Departamento
            {
                nombre = dto.nombre,
                descripcion = dto.descripcion,
            };
            var saved = await _repo.AddAsync(entity);

            return MapToResponseDTO(saved);
        }

        public async Task<bool> UpdateAsync(int idDepartamento, DepartamentoUpdateDTO dto)
        {
            var current = await _repo.GetByIdAsync(idDepartamento);

            if (current == null)
            {
                throw new NotFoundException($"Departamento con ID {idDepartamento} no encontrado para la actualización.");
            }
            current.nombre = dto.nombre.Trim();
            current.descripcion = dto.descripcion.Trim();

            return await _repo.UpdateAsync(current);
        }

        public async Task<bool> DeleteAsync(int idDepartamento)
        {
            bool wasDeleted = await _repo.DeleteAsync(idDepartamento);

            if (!wasDeleted)
            {
                throw new NotFoundException($"Departamento con ID {idDepartamento} no existe para ser eliminado.");
            }
            return true;
        }
    }
}
