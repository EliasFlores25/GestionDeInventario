using GestionDeInventario.DTOs.DistribucionDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class DistribucionService : IDistribucionService
    {
        private readonly IDistribucionRepository _repository;
        public DistribucionService(IDistribucionRepository repository)
        {
            _repository = repository;
        }
        private DistribucionResponseDTO MapToResponseDTO(Distribucion x)
        {
            return new DistribucionResponseDTO
            {
                IdDistribucion = x.IdDistribucion,
                NumeroDistribucion = x.NumeroDistribucion,
                UsuarioId = x.UsuarioId,
                Usuario = x.Usuario,
                EmpleadoId = x.EmpleadoId,
                Empleado = x.Empleado,
                FechaSalida = x.FechaSalida,
                Motivo = x.Motivo,
                MontoTotalDistribucion = x.MontoTotalDistribucion
            };
        }
        public IQueryable<DistribucionResponseDTO> GetQueryable()
        {
            return _repository.GetQueryable().Select(x => new DistribucionResponseDTO
            {
                IdDistribucion=x.IdDistribucion,
                NumeroDistribucion = x.NumeroDistribucion,
                UsuarioId = x.UsuarioId,
                EmpleadoId = x.EmpleadoId,
                FechaSalida = x.FechaSalida,
                Motivo = x.Motivo,
            });

        }
        public async Task<List<DistribucionResponseDTO>> GetAllAsync()=>
             (await _repository.GetAllAsync()).Select(MapToResponseDTO).ToList();
        public async Task<DistribucionResponseDTO> GetByIdAsync(int idDistribucion)
        {
            var x = await _repository.GetByIdAsync(idDistribucion);
            if (x == null)
            {
                throw new NotFoundException($"Distribución con ID {idDistribucion} no encontrado.");
            }
            return MapToResponseDTO(x);
        }
        public async Task<DistribucionResponseDTO> AddAsync(DistribucionCreateDTO dto)
        {
            var entity = new Distribucion
            {
                NumeroDistribucion = dto.NumeroDistribucion,
                UsuarioId = dto.UsuarioId,
                EmpleadoId = dto.EmpleadoId,
                FechaSalida = dto.FechaSalida,
                Motivo = dto.Motivo,
            };
            var saved = await _repository.AddAsync(entity);
            return MapToResponseDTO(saved);
        }
        public async Task<bool> UpdateAsync(int idDistribucion, DistribucionUpdateDTO dto)
        {
            var current = await _repository.GetByIdAsync(idDistribucion);

            current.NumeroDistribucion = dto.NumeroDistribucion.Trim();
            current.UsuarioId = dto.UsuarioId;
            current.EmpleadoId = dto.EmpleadoId;
            current.FechaSalida = dto.FechaSalida;
            current.Motivo = dto.Motivo;
            return await _repository.UpdateAsync(current);
        }
        public async Task<bool> DeleteAsync(int idDistribucion)
        {
            bool wasDeleted = await _repository.DeleteAsync(idDistribucion);

            if (!wasDeleted)
            {
                throw new NotFoundException($"Distribución con ID {idDistribucion} no existe para ser eliminado.");
            }
            return true;
        }
    }
}
