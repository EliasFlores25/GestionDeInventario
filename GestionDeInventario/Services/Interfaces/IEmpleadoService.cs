using GestionDeInventario.DTOs.EmpleadoDTOs;

namespace GestionDeInventario.Services.Interfaces
{
    public interface IEmpleadoService
    {
        IQueryable<EmpleadoResponseDTO> GetQueryable();
        Task<List<EmpleadoResponseDTO>> GetAllAsync();
        Task<EmpleadoResponseDTO> GetByIdAsync(int idEmpleado);
        Task<EmpleadoResponseDTO> AddAsync(EmpleadoCreateDTO dto);
        Task<bool> UpdateAsync(int idEmpleado, EmpleadoUptadeDTO dto);
        Task<bool> DeleteAsync(int idEmpleado);
    }
}

