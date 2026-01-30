using GestionDeInventario.DTOs.DetalleCompraDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;
using GestionDeInventario.Views.Empleado;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Services.Implementations
{
    public class DetalleCompraService : IDetalleCompraService
    {
        private readonly IDetalleCompraRepository _repo;
        public DetalleCompraService(IDetalleCompraRepository repo)
        {
            _repo = repo;
        }

        private DetalleCompraResponseDTO MapToResponseDTO(DetalleCompra x)
        {
            return new DetalleCompraResponseDTO
            {
                IdDetalleCompra = x.IdDetalleCompra,
                CompraId = x.CompraId,
                Compra = x.Compra,
                ProductoId = x.ProductoId,
                Producto = x.Producto,
                PrecioUnitarioCosto = x.PrecioUnitarioCosto,
                Cantidad = x.Cantidad,
                Subtotal = x.Subtotal,
            };
        }
        public IQueryable<DetalleCompraResponseDTO> GetQueryable()
        {
            return _repo.GetQueryable().Select(x => new DetalleCompraResponseDTO
            {
                IdDetalleCompra = x.IdDetalleCompra,
                CompraId = x.CompraId,
                ProductoId = x.ProductoId,
                PrecioUnitarioCosto = x.PrecioUnitarioCosto,
                Cantidad = x.Cantidad,
                Subtotal = x.Subtotal,
            });
        }
        public async Task<List<DetalleCompraResponseDTO>> GetAllAsync() =>
            (await _repo.GetAllAsync()).Select(MapToResponseDTO).ToList();
        public async Task<DetalleCompraResponseDTO> GetByIdAsync(int idDetalleCompra)
        {
            var x = await _repo.GetByIdAsync(idDetalleCompra);

            if (x == null)
            {
                throw new NotFoundException($"Detalle Compra con ID {idDetalleCompra} no encontrado.");
            }
            return MapToResponseDTO(x);

        }
        public async Task<DetalleCompraResponseDTO> AddAsync(DetalleCompraCreateDTO dto)
        {

            // 1. Mapeo de DTO a Modelo
            var detalleCompra = new DetalleCompra
            {
                CompraId = dto.CompraId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                PrecioUnitarioCosto = dto.PrecioUnitarioCosto
            };
            try
            {
                var saved = await _repo.AddAsync(detalleCompra);
                return MapToResponseDTO(saved);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("Error al procesar la compra: " + ex.Message);
            }
        }
        public async Task<bool> UpdateAsync(int idDetalleCompra, DetalleCompraUpdateDTO dto)
        {
            var current = await _repo.GetByIdAsync(idDetalleCompra);
            if (current == null) throw new NotFoundException("No existe la compra.");

            current.CompraId = dto.CompraId;
            current.ProductoId = dto.ProductoId;
            current.Cantidad = dto.Cantidad;
            current.PrecioUnitarioCosto = dto.PrecioUnitarioCosto;
            try
            {
                return await _repo.UpdateAsync(current);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("No se pudo actualizar el stock: " + ex.Message);
            }
        }
        public async Task<bool> DeleteAsync(int idDetalleCompra)
        {
            var existente = await _repo.GetByIdAsync(idDetalleCompra);
            if (existente == null) throw new NotFoundException("No existe la compra.");

            try
            {
                return await _repo.DeleteAsync(idDetalleCompra);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("Error al eliminar registro: " + ex.Message);
            }
        }

        // Método para Excel
        public IQueryable<DetalleCompraExcelDTO> GetQueryableForExcel()
        {
            return _repo.GetQueryable()
                .Select(x => new DetalleCompraExcelDTO
                {
                    IdDetalleCompra = x.IdDetalleCompra,

                    NumeroFactura = x.Compra != null
                        ? x.Compra.NumeroFactura : $"ID: {x.CompraId}",

                    nombre = x.Producto != null
                        ? x.Producto.nombre : $"ID: {x.ProductoId}",

                    Cantidad = x.Cantidad,
                    PrecioUnitarioCosto = x.PrecioUnitarioCosto,
                    Subtotal = x.Subtotal,
                })
                .AsQueryable();
        }
    }
}