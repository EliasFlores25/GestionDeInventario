using GestionDeInventario.DTOs.DepartamentoDTOs;
using GestionDeInventario.DTOs.ProveedorDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;
        public ProveedorService(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
        }
        private ProveedorResponseDTO MapToResponseDTO(Proveedor x)
        {
            return new ProveedorResponseDTO
            {
                idProveedor = x.idProveedor,
                nombreEmpresa = x.nombreEmpresa,
                direccion = x.direccion,
                telefono = x.telefono,
                email = x.email,
                estado = x.estado,
            };
        }
        public IQueryable<ProveedorResponseDTO> GetQueryable()
        {
            return _proveedorRepository.GetQueryable().Select(x => new ProveedorResponseDTO
            {
                idProveedor = x.idProveedor,
                nombreEmpresa = x.nombreEmpresa,
                direccion = x.direccion,
                telefono = x.telefono,
                email = x.email,
                estado = x.estado,
            });
        }
        public async Task<List<ProveedorResponseDTO>> GetAllAsync() =>
            (await _proveedorRepository.GetAllAsync()).Select(MapToResponseDTO).ToList();
        public async Task<ProveedorResponseDTO> GetByIdAsync(int idProveedor)
        {
            {
                var x = await _proveedorRepository.GetByIdAsync(idProveedor);
                if (x == null)
                {
                    throw new NotFoundException($"Proveedor con ID {idProveedor} no encontrado.");
                }
                return MapToResponseDTO(x);
            }
        }
        public async Task<ProveedorResponseDTO> AddAsync(ProveedorCreateDTO dto)
        {
            var entity = new Proveedor
            {
                nombreEmpresa = dto.nombreEmpresa,
                direccion = dto.direccion,
                telefono = dto.telefono,
                email = dto.email,
                estado = dto.estado,
            };
            var saved = await _proveedorRepository.AddAsync(entity);

            return MapToResponseDTO(saved);
        }
        public async Task<bool> UpdateAsync(int idProveedor, ProveedorUpdateDTO dto)
        {

            var current = await _proveedorRepository.GetByIdAsync(idProveedor);

            if (current == null)
            {
                throw new NotFoundException($"Proveedor con ID {idProveedor} no encontrado para la actualización.");
            }
            current.nombreEmpresa = dto.nombreEmpresa.Trim();
            current.direccion = dto.direccion.Trim();
            current.telefono = dto.telefono.Trim();
            current.email = dto.email.Trim();
            current.estado = dto.estado.Trim();

            return await _proveedorRepository.UpdateAsync(current);
        }
        public async Task<bool> DeleteAsync(int idProveedor)
        {
            bool wasDeleted = await _proveedorRepository.DeleteAsync(idProveedor);

            if (!wasDeleted)
            {
                throw new NotFoundException($"Proveedor con ID {idProveedor} no existe para ser eliminado.");
            }
            return true;
        }
    }
}