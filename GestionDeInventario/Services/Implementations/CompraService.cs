using GestionDeInventario.DTOs.CompraDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class CompraService : ICompraService
    {
        private readonly ICompraRepository _repository;
        public CompraService(ICompraRepository repository)
        {
            _repository = repository;
        }
        private CompraResponseDTO MapToResponseDTO(Compra x)
        {
            return new CompraResponseDTO
            {
                IdCompra = x.IdCompra,
                NumeroFactura = x.NumeroFactura,
                UsuarioId = x.UsuarioId,
                Usuario = x.Usuario,
                ProveedorId = x.ProveedorId,
                Proveedor = x.Proveedor,
                FechaCompra = x.FechaCompra,
                MontoTotalCompra = x.MontoTotalCompra,
            };
        }
        public IQueryable<CompraResponseDTO> GetQueryable()
        {
            return _repository.GetQueryable().Select(x => new CompraResponseDTO
            {
                IdCompra = x.IdCompra,
                NumeroFactura = x.NumeroFactura,
                UsuarioId = x.UsuarioId,
                ProveedorId = x.ProveedorId,
                FechaCompra = x.FechaCompra,
                MontoTotalCompra = x.MontoTotalCompra,
            });
        }
        public async Task<List<CompraResponseDTO>> GetAllAsync()
        => (await _repository.GetAllAsync()).Select(MapToResponseDTO).ToList();

        public async Task<CompraResponseDTO> GetByIdAsync(int idCompra)
        {
            var x =await _repository.GetByIdAsync(idCompra);
            if (x == null)
            {
                throw new NotFoundException($"Empleado con id {idCompra} no encontrado.");
            }
            return MapToResponseDTO(x);
        }
        public async Task<CompraResponseDTO> AddAsync(CompraCreateDTO dto)
        {
            var entity = new Compra
            {
                NumeroFactura = dto.NumeroFactura,
                UsuarioId = dto.UsuarioId,
                ProveedorId = dto.ProveedorId,
                FechaCompra = dto.FechaCompra,
            };
            var saved = await _repository.AddAsync(entity);
            return MapToResponseDTO(saved);
        }
        public async Task<bool> DeleteAsync(int idCompra)
        {
            bool wasDeleted = await _repository.DeleteAsync(idCompra);
            if (!wasDeleted)
            {
                throw new NotFoundException($"Compra con ID {idCompra} no existe para ser eliminado.");
            }
            return true;
        }
        public async Task<bool> UpdateAsync(int idCompra, CompraUpdateDTO dto)
        {
            var current = await _repository.GetByIdAsync(idCompra);

            current.NumeroFactura = dto.NumeroFactura.Trim();
            current.UsuarioId = dto.UsuarioId;
            current.ProveedorId = dto.ProveedorId;
            current.FechaCompra = dto.FechaCompra;

            return await _repository.UpdateAsync(current);
        }
    }
}
