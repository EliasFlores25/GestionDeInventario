using GestionDeInventario.DTOs.ProveedorDTOs;

namespace GestionDeInventario.Services.Interfaces
{
    public interface IProveedorService
    {
        IQueryable<ProveedorResponseDTO> GetQueryable();
        Task<List<ProveedorResponseDTO>> GetAllAsync();
        Task<ProveedorResponseDTO> GetByIdAsync(int idProveedor);
        Task<ProveedorResponseDTO> AddAsync(ProveedorCreateDTO dto);
        Task<bool> UpdateAsync(int idProveedor, ProveedorUpdateDTO dto);
        Task<bool> DeleteAsync(int idProveedor);
    }
}
