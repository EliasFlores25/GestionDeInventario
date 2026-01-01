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
                NumeroDistribucion = x.NumeroDistribucion,
                UsuarioId = x.UsuarioId,
                Usuario = x.Usuario,
                EmpleadoId = x.EmpleadoId,
                Empleado = x.Empleado,
                ProductoId = x.ProductoId,
                Producto = x.Producto,
                Cantidad = x.Cantidad,
                FechaSalida = x.FechaSalida,
                Motivo = x.Motivo ?? string.Empty,
                PrecioCostoUnitario = x.PrecioCostoUnitario,
                MontoTotal = x.MontoTotal,
            };
        }
        public IQueryable<DetalleDistribucionResponseDTO> GetQueryable()
        {
            return _repository.GetQueryable().Select(x => new DetalleDistribucionResponseDTO
            {
                IdDetalleDistribucion = x.IdDetalleDistribucion,
                NumeroDistribucion = x.NumeroDistribucion,
                UsuarioId = x.UsuarioId,
                EmpleadoId = x.EmpleadoId,
                ProductoId = x.ProductoId,
                Cantidad = x.Cantidad,
                FechaSalida = x.FechaSalida,
                Motivo = x.Motivo,
                PrecioCostoUnitario = x.PrecioCostoUnitario,
                MontoTotal = x.MontoTotal,
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
                NumeroDistribucion = dto.NumeroDistribucion,
                UsuarioId = dto.UsuarioId,
                EmpleadoId = dto.EmpleadoId,
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                FechaSalida = dto.FechaSalida,
                Motivo = dto.Motivo,
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

            current.NumeroDistribucion = dto.NumeroDistribucion.Trim();
            current.EmpleadoId = dto.EmpleadoId;
            current.ProductoId = dto.ProductoId;
            current.Cantidad = dto.Cantidad;
            current.FechaSalida = dto.FechaSalida;
            current.Motivo = dto.Motivo;
            current.UsuarioId = dto.UsuarioId;
            //current.PrecioCostoUnitario = dto.PrecioCostoUnitario;
            current.MontoTotal = dto.MontoTotal;
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





        // NUEVO: Método para Excel
        public IQueryable<DetalleDistribucionExcelDTO> GetQueryableForExcel()
        {
            return _repository.GetQueryable()
                .Select(x => new DetalleDistribucionExcelDTO
                {
                    IdDetalleDistribucion = x.IdDetalleDistribucion,
                    NumeroDistribucion = x.NumeroDistribucion,
                    FechaSalida = x.FechaSalida,
                    NombreEmpleado = x.Empleado != null
                        ? $"{x.Empleado.nombre} {x.Empleado.apellido}"
                        : $"ID: {x.EmpleadoId}",
                    NombreProducto = x.Producto != null
                        ? x.Producto.nombre
                        : $"ID: {x.ProductoId}",
                    Cantidad = x.Cantidad,
                    Motivo = x.Motivo ?? "Sin motivo",
                    PrecioCostoUnitario = x.PrecioCostoUnitario,
                    MontoTotal = x.MontoTotal,
                    UsuarioRegistro = x.Usuario != null
                        ? x.Usuario.nombre
                        : $"ID: {x.UsuarioId}"
                })
                .AsQueryable();




        }
    }
}