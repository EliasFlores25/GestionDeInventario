using GestionDeInventario.DTOs.DetalleDistribucionDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class DetalleDistribucionService : IDetalleDistribucionService
    {
        public readonly IDetalleDistribucionRepository _repository;
        public DetalleDistribucionService(IDetalleDistribucionRepository repository)
        {
            _repository = repository;
        }
        private DetalleDistribucionResponseDTO MapToResponseDTO(DetalleDistribucion x)
        {
            return new DetalleDistribucionResponseDTO
            {
                IdDetalleDistribucion = x.IdDetalleDistribucion,
                DistribucionId = x.DistribucionId,
                Distribucion = x.Distribucion,
                ProductoId = x.ProductoId,
                Producto = x.Producto,
                Cantidad = x.Cantidad,
                PrecioCostoUnitario = x.PrecioCostoUnitario,
                Subtotal = x.Subtotal,
            };
        }
        public IQueryable<DetalleDistribucionResponseDTO> GetQueryable()
        {
            return _repository.GetQueryable().Select(x => new DetalleDistribucionResponseDTO
            {
                IdDetalleDistribucion = x.IdDetalleDistribucion,
                DistribucionId = x.DistribucionId,
                ProductoId = x.ProductoId,
                Cantidad = x.Cantidad,
                PrecioCostoUnitario = x.PrecioCostoUnitario,
                Subtotal = x.Subtotal,
            });
        }
        public async Task<List<DetalleDistribucionResponseDTO>> GetAllAsync() =>
            (await _repository.GetAllAsync()).Select(MapToResponseDTO).ToList();

        public async Task<DetalleDistribucionResponseDTO> GetByIdAsync(int idDetalleDistribucion)
        {
            var x = await _repository.GetByIdAsync(idDetalleDistribucion);
            if (x == null)
            {
                throw new NotFoundException($"Detalle de la distribución con ID {idDetalleDistribucion} no encontrado.");
            }
            return MapToResponseDTO(x);
        }
        public async Task<DetalleDistribucionResponseDTO> AddAsync(DetalleDistribucionCreateDTO dto)
        {
            // 1. Mapeo de DTO a Modelo
            var detalleDistribucion = new DetalleDistribucion
            {
                DistribucionId = dto.DistribucionId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
            };
            try
            {
                var saved = await _repository.AddAsync(detalleDistribucion);
                return MapToResponseDTO(saved);
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException?.Message ?? ex.Message;
                throw new BusinessRuleException($"No se pudo procesar la distribución: {mensajeError}");
            }
        }
        public async Task<bool> UpdateAsync(int idDetalleDistribucion, DetalleDistribucionUpdateDTO dto)
        {
            var current = await _repository.GetByIdAsync(idDetalleDistribucion);
            if (current == null) throw new NotFoundException("No existe la distribución.");

            current.DistribucionId = dto.DistribucionId;
            current.ProductoId = dto.ProductoId;
            current.Cantidad = dto.Cantidad;
            try
            {
                return await _repository.UpdateAsync(current);
            }
            catch (Exception ex)
            {
                string mensajeError = ex.InnerException?.Message ?? ex.Message;
                throw new BusinessRuleException($"Error al actualizar la distribución: {mensajeError}");
            }
        }
        public async Task<bool> DeleteAsync(int idDetalleDistribucion)
        {
            var existente = await _repository.GetByIdAsync(idDetalleDistribucion);
            if (existente == null) throw new NotFoundException("No existe la distribución.");

            try
            {
                return await _repository.DeleteAsync(idDetalleDistribucion);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleException("Error al eliminar registro: " + ex.Message);
            }
        }

        //  Método para Excel
        public IQueryable<DetalleDistribucionExcelDTO> GetQueryableForExcel()
        {
            return _repository.GetQueryable()
                .Select(x => new DetalleDistribucionExcelDTO
                {
                    IdDetalleDistribucion = x.IdDetalleDistribucion,

                    NumeroDistribucion = x.Distribucion != null
                        ? x.Distribucion.NumeroDistribucion : $"ID: {x.DistribucionId}",

                    nombre = x.Producto != null
                        ? x.Producto.nombre : $"ID: {x.ProductoId}",

                    Cantidad = x.Cantidad,

                    PrecioCostoUnitario = x.PrecioCostoUnitario,

                    Subtotal = x.Subtotal,
                })
                .AsQueryable();
        }
    }
}