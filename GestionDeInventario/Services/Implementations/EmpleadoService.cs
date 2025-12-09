using GestionDeInventario.DTOs.EmpleadoDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repo;
        public EmpleadoService(IEmpleadoRepository repo)
        {
            _repo = repo;
        }
        private EmpleadoResponseDTO MapToResponseDTO(Empleado x)
        {
            return new EmpleadoResponseDTO
            {
                idEmpleado = x.idEmpleado,
                nombre = x.nombre,
                apellido = x.apellido,
                edad = x.edad,
                genero = x.genero,
                telefono = x.telefono,
                direccion = x.direccion,
                departamentoId = x.departamentoId,
                estado = x.estado,
            };
        }
        public IQueryable<EmpleadoResponseDTO> GetQueryable()
        {
            return _repo.GetQueryable().Select(x => new EmpleadoResponseDTO
            {
                idEmpleado = x.idEmpleado,
                nombre = x.nombre,
                apellido = x.apellido,
                edad = x.edad,
                genero = x.genero,
                telefono = x.telefono,
                direccion = x.direccion,
                departamentoId = x.departamentoId,
                estado = x.estado,
            });
        }
        public async Task<List<EmpleadoResponseDTO>> GetAllAsync() =>
            (await _repo.GetAllAsync()).Select(MapToResponseDTO).ToList();
        public async Task<EmpleadoResponseDTO> GetByIdAsync(int idEmpleado)
        {
            var x = await _repo.GetByIdAsync(idEmpleado);
            if (x == null)
            {
                throw new NotFoundException($"Empleado con ID {idEmpleado} no encontrado.");
            }
            return MapToResponseDTO(x);
        }
        public async Task<EmpleadoResponseDTO> AddAsync(EmpleadoCreateDTO dto)
        {
            if (dto.edad < 18 || dto.edad > 100)
            {
                throw new BusinessRuleException("La edad debe ser igual o mayor a 18 y menor o igual a 100 años.");
            }

            var entity = new Empleado
            {
                nombre = dto.nombre,
                apellido = dto.apellido,
                edad = dto.edad,
                genero = dto.genero,
                telefono = dto.telefono,
                direccion = dto.direccion,
                departamentoId = dto.departamentoId,
                estado = dto.estado,
            };

            var saved = await _repo.AddAsync(entity);

            return MapToResponseDTO(saved);
        }
        public async Task<bool> UpdateAsync(int idEmpleado, EmpleadoUptadeDTO dto)
        {
            var current = await _repo.GetByIdAsync(idEmpleado);

            if (dto.edad < 18 || dto.edad > 100)
            {
                throw new BusinessRuleException("La edad debe ser igual o mayor a 18 y menor o igual a 100 años.");
            }

            current.nombre = dto.nombre.Trim();
            current.apellido = dto.apellido.Trim();
            current.edad = dto.edad;
            current.genero = dto.genero.Trim();
            current.telefono = dto.telefono.Trim();
            current.direccion = dto.direccion.Trim();
            current.departamentoId = dto.departamentoId;
            current.estado = dto.estado.Trim();

            return await _repo.UpdateAsync(current);
        }
        public async Task<bool> DeleteAsync(int idEmpleado)
        {
            bool wasDeleted = await _repo.DeleteAsync(idEmpleado);

            if (!wasDeleted)
            {
                throw new NotFoundException($"Empleado con ID {idEmpleado} no existe para ser eliminado.");
            }
            return true;
        }
    }
}
