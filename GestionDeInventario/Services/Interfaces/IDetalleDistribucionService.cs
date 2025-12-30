using GestionDeInventario.DTOs.DetalleDistribucionDTOs;

namespace GestionDeInventario.Services.Interfaces
{
    public interface IDetalleDistribucionService
    {
        IQueryable<DetalleDistribucionResponseDTO> GetQueryable();
        Task<List<DetalleDistribucionResponseDTO>> GetAllAsync();
        Task<DetalleDistribucionResponseDTO> GetByIdAsync(int idDetalleDistribucion);
        Task<DetalleDistribucionResponseDTO> AddAsync(DetalleDistribucionCreateDTO dto);
        Task<bool> UpdateAsync(int idDetalleDistribucion, DetalleDistribucionUpdateDTO dto);
        Task<bool> DeleteAsync(int idDetalleDistribucion);
    }
}
