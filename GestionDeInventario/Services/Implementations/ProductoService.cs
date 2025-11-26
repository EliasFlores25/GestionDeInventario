using GestionDeInventario.DTOs.ProductoDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;
using System.Linq;

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

        // --- MÉTODOS DE LECTURA (Mapeo Manual) ---

        public IQueryable<ProductoResponseDTO> GetQueryable()
        {
            // Mapeo manual con Select para ejecución eficiente en la BD
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

        //public async Task<ProductoResponseDTO?> GetByIdAsync(int idProducto)
        //{
        //    var x = await _repo.GetByIdAsync(idProducto);
        //    // El servicio aún devuelve 'null' si no existe; el controlador puede decidir qué hacer.
        //    return x == null ? null : MapToResponseDTO(x);
        //}
        public async Task<ProductoResponseDTO> GetByIdAsync(int idProducto)
        {
            var x = await _repo.GetByIdAsync(idProducto);
            if (x == null)
            {
                // 🚨 Ahora el Servicio lanza la excepción
                throw new NotFoundException($"Producto con ID {idProducto} no encontrado.");
            }
            return MapToResponseDTO(x);
        }

        // --- MÉTODOS DE ESCRITURA (Con Validación y Errores) ---

        public async Task<ProductoResponseDTO> AddAsync(ProductoCreateDTO dto)
        {
            // 🛡️ 1. Validación de Negocio (Ejemplo 1: Precio)
            if (dto.precio <= 0)
            {
                throw new BusinessRuleException("El precio unitario debe ser mayor que cero.");
            }
            // 🛡️ 2. Validación de Negocio (Ejemplo 2: Stock inicial)
            if (dto.cantidadStock < 0)
            {
                throw new BusinessRuleException("El stock inicial no puede ser negativo.");
            }

            // Mapeo Manual de DTO a Entidad
            var entity = new Producto
            {
                nombre = dto.nombre,
                descripcion = dto.descripcion,
                cantidadStock = dto.cantidadStock,
                unidadMedida = dto.unidadMedida,
                precio = dto.precio,
                estado = dto.estado, // Asumimos estado inicial por defecto
            };

            var saved = await _repo.AddAsync(entity);

            // Mapeo Manual de Entidad guardada a DTO de respuesta
            return MapToResponseDTO(saved);
        }

        public async Task<bool> UpdateAsync(int idProducto, ProductoUpdateDTO dto)
        {
            var current = await _repo.GetByIdAsync(idProducto);

            // 🚨 Manejo de Errores: Lanzar excepción generalizada si no se encuentra
            if (current == null)
            {
                throw new NotFoundException($"Producto con ID {idProducto} no encontrado para la actualización.");
            }

            // 🛡️ Validación de Negocio: No permitir stock negativo en la actualización
            if (dto.cantidadStock < 0)
            {
                throw new BusinessRuleException("La cantidad en stock no puede ser negativa.");
            }

            // Mapeo Manual de DTO a Entidad existente (Actualización)
            current.nombre = dto.nombre.Trim(); // Mantener el Trim si es requerido
            current.descripcion = dto.descripcion.Trim();
            current.cantidadStock = dto.cantidadStock;
            current.unidadMedida = dto.unidadMedida.Trim();
            current.precio = dto.precio;
            current.estado = dto.estado.Trim();

            return await _repo.UpdateAsync(current);
        }

        //public async Task<bool> DeleteAsync(int idProducto)
        //{
        //    // Opcional: Para asegurar que el servicio lanza una excepción si no existe, 
        //    // se puede hacer una búsqueda previa, aunque el repositorio maneja el 'false'.
        //    var existing = await _repo.GetByIdAsync(idProducto);
        //    if (existing == null)
        //    {
        //        throw new NotFoundException($"Producto con ID {idProducto} no existe para ser eliminado.");
        //    }

        //    // Si la regla de negocio permite eliminarlo, se delega al repositorio
        //    return await _repo.DeleteAsync(idProducto);
        //}
        public async Task<bool> DeleteAsync(int idProducto)
        {
            // Delegamos la eliminación al repositorio y verificamos el resultado.
            bool wasDeleted = await _repo.DeleteAsync(idProducto);

            if (!wasDeleted)
            {
                // El repositorio devolvió false, asumimos que no existía.
                throw new NotFoundException($"Producto con ID {idProducto} no existe para ser eliminado.");
            }
            return true; // Éxito en la eliminación
        }
    }
}