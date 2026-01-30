using GestionDeInventario.DTOs.CompraDTOs;

namespace GestionDeInventario.Services.Interfaces
{
    public interface ICompraService
    {
        IQueryable<CompraResponseDTO> GetQueryable();
        Task<List<CompraResponseDTO>> GetAllAsync();
        Task<CompraResponseDTO> GetByIdAsync(int idCompra);
        Task<bool> DeleteAsync(int idCompra);
        Task<bool> UpdateAsync(int idCompra, CompraUpdateDTO dto);
        Task<CompraResponseDTO> AddAsync(CompraCreateDTO dto);
    }
}
