using GestionDeInventario.DTOs.ProductoDTOs;

namespace GestionDeInventario.Services.Interfaces
{
    public interface IProductoService
    {
        IQueryable<ProductoResponseDTO> GetQueryable();
        Task<List<ProductoResponseDTO>> GetAllAsync();
        Task<ProductoResponseDTO> GetByIdAsync(int idProducto);
        Task<ProductoResponseDTO> AddAsync(ProductoCreateDTO dto);
        Task<bool> UpdateAsync(int idProducto, ProductoUpdateDTO dto);
        Task<bool> DeleteAsync(int idProducto);
    }
}
