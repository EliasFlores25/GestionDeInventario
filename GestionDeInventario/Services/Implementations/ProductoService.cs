using GestionDeInventario.DTOs.ProductoDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repo;
        public ProductoService(IProductoRepository repo)
        {
            _repo = repo;
        }

        // --- Método Auxiliar de Mapeo Manual (Reutilización) ---
        private ProductoResponseDTO MapToResponseDTO(Producto x)
        {
            return new ProductoResponseDTO
            {
                idProducto = x.idProducto,
                nombre = x.nombre,
                descripcion = x.descripcion,
                cantidadStock = x.cantidadStock,
                unidadMedida = x.unidadMedida,
                precio = x.precio,
                estado = x.estado,
            };
        }

        public IQueryable<ProductoResponseDTO> GetQueryable()
        {
            return _repo.GetQueryable().Select(x => new ProductoResponseDTO
            {
                idProducto = x.idProducto,
                nombre = x.nombre,
                descripcion = x.descripcion,
                cantidadStock = x.cantidadStock,
                unidadMedida = x.unidadMedida,
                precio = x.precio,
                estado = x.estado,
            });
        }

        public async Task<List<ProductoResponseDTO>> GetAllAsync() =>
            (await _repo.GetAllAsync()).Select(MapToResponseDTO).ToList();

        public async Task<ProductoResponseDTO> GetByIdAsync(int idProducto)
        {
            var x = await _repo.GetByIdAsync(idProducto);
            if (x == null)
            {
                throw new NotFoundException($"Producto con ID {idProducto} no encontrado.");
            }
            return MapToResponseDTO(x);
        }

        public async Task<ProductoResponseDTO> AddAsync(ProductoCreateDTO dto)
        {
            if (dto.precio <= 0)
            {
                throw new BusinessRuleException("El precio unitario debe ser mayor que cero.");
            }
            if (dto.cantidadStock < 0)
            {
                throw new BusinessRuleException("El stock inicial no puede ser negativo.");
            }

            var entity = new Producto
            {
                nombre = dto.nombre,
                descripcion = dto.descripcion,
                cantidadStock = dto.cantidadStock,
                unidadMedida = dto.unidadMedida,
                precio = dto.precio,
                estado = dto.estado,
            };

            var saved = await _repo.AddAsync(entity);

            return MapToResponseDTO(saved);
        }

        public async Task<bool> UpdateAsync(int idProducto, ProductoUpdateDTO dto)
        {
            var current = await _repo.GetByIdAsync(idProducto);

            if (current == null)
            {
                throw new NotFoundException($"Producto con ID {idProducto} no encontrado para la actualización.");
            }

            if (dto.cantidadStock < 0)
            {
                throw new BusinessRuleException("La cantidad en stock no puede ser negativa.");
            }

            current.nombre = dto.nombre.Trim();
            current.descripcion = dto.descripcion.Trim();
            current.cantidadStock = dto.cantidadStock;
            current.unidadMedida = dto.unidadMedida.Trim();
            current.precio = dto.precio;
            current.estado = dto.estado.Trim();

            return await _repo.UpdateAsync(current);
        }

        public async Task<bool> DeleteAsync(int idProducto)
        {
            bool wasDeleted = await _repo.DeleteAsync(idProducto);

            if (!wasDeleted)
            {
                throw new NotFoundException($"Producto con ID {idProducto} no existe para ser eliminado.");
            }
            return true;
        }
    }
}