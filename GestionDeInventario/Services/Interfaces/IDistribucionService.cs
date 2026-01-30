using GestionDeInventario.DTOs.DistribucionDTOs;
using GestionDeInventario.DTOs.EmpleadoDTOs;

namespace GestionDeInventario.Services.Interfaces
{
    public interface IDistribucionService
    {
        IQueryable<DistribucionResponseDTO> GetQueryable();
        Task<List<DistribucionResponseDTO>> GetAllAsync();
        Task<DistribucionResponseDTO> GetByIdAsync(int idDistribucion);
        Task<DistribucionResponseDTO> AddAsync(DistribucionCreateDTO dto);
        Task<bool> UpdateAsync(int idDistribuciob, DistribucionUpdateDTO dto);
        Task<bool> DeleteAsync(int idDistribucion);

    }
}
