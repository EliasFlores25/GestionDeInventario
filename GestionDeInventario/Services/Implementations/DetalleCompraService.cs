using GestionDeInventario.DTOs.DetalleCompra;
using GestionDeInventario.DTOs.DetalleCompraDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

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
                idDetalleCompra = x.idDetalleCompra,
                numeroFactura = x.numeroFactura,
                usuarioId = x.usuarioId,
                proveedorId = x.proveedorId,
                productoId = x.productoId,
                cantidad = x.cantidad,
                precioUnitarioCosto = x.precioUnitarioCosto,
                montoTotal = x.montoTotal,
                fechaCompra = x.fechaCompra,
            };
        }
        public IQueryable<DetalleCompraResponseDTO> GetQueryable()
        {
            return _repo.GetQueryable().Select(x => new DetalleCompraResponseDTO
            {
                idDetalleCompra = x.idDetalleCompra,
                numeroFactura = x.numeroFactura,
                usuarioId = x.usuarioId,
                proveedorId = x.proveedorId,
                productoId = x.productoId,
                cantidad = x.cantidad,
                precioUnitarioCosto = x.precioUnitarioCosto,
                montoTotal = x.montoTotal,
                fechaCompra = x.fechaCompra,
            });
        }
        public async Task<List<DetalleCompraResponseDTO>> GetAllAsync() =>
            (await _repo.GetAllAsync()).Select(MapToResponseDTO).ToList();
        public async Task<DetalleCompraResponseDTO> GetByIdAsync(int idDetalleCompra)
        {
            var x = await _repo.GetByIdAsync(idDetalleCompra);
            if (x == null)
            {
                throw new NotFoundException($"Detalle de la compra con ID {idDetalleCompra} no encontrado.");
            }
            return MapToResponseDTO(x);
        }
        public async Task<DetalleCompraResponseDTO> AddAsync(DetalleCompraCreateDTO dto)
        {
            // 1. Mapeo de DTO a Modelo
            var detalleCompra = new DetalleCompra
            {
                numeroFactura = dto.numeroFactura,
                cantidad = dto.cantidad,
                precioUnitarioCosto = dto.precioUnitarioCosto,
                montoTotal = dto.montoTotal,
                fechaCompra = dto.fechaCompra,
                usuarioId = dto.usuarioId,
                proveedorId = dto.proveedorId,
                productoId = dto.productoId,
            };
            try
            {
                // 2. Guardar en BD
                // Al ejecutar esto, el Trigger de SQL se dispara automáticamente
                // y actualiza el stock y precio en la tabla Producto.
                var saved = await _repo.AddAsync(detalleCompra);

                // 3. Retornar el mapeo (incluyendo el montoTotal calculado por la DB)
                return MapToResponseDTO(saved);
            }
            catch (Exception ex)
            {
                // Si el Trigger lanza un error (ej. validación), lo capturamos aquí
                throw new BusinessRuleException("Error al procesar la compra: " + ex.Message);
            }
        }
        public async Task<bool> UpdateAsync(int idDetalleCompra, DetalleCompraUpdateDTO dto)
        {
            var current = await _repo.GetByIdAsync(idDetalleCompra);
            if (current == null) throw new NotFoundException("No existe la compra.");

            current.numeroFactura = dto.numeroFactura.Trim();
            current.cantidad = dto.cantidad;
            current.precioUnitarioCosto = dto.precioUnitarioCosto;
            current.montoTotal = dto.montoTotal;
            current.fechaCompra = dto.fechaCompra;
            current.usuarioId = dto.usuarioId;
            current.proveedorId = dto.proveedorId;
            current.productoId = dto.productoId;

            try
            {
                // El Trigger 'trg_actualizar_stock_compra' de la DB hará el ajuste 
                // de restar la cantidad vieja y sumar la nueva automáticamente.
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
                // El Trigger 'trg_eliminar_stock_compra' restará el stock que se había ingresado.
                return await _repo.DeleteAsync(idDetalleCompra);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("Error al eliminar registro: " + ex.Message);
            }
        }
    }
}