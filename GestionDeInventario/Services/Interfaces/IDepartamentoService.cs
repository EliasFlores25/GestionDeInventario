using GestionDeInventario.DTOs.DepartamentoDTOs;
using GestionDeInventario.DTOs.ProductoDTOs;
using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IDepartamentoService
    {
        IQueryable<DepartamentoResponseDTO> GetQueryable();
        Task<List<DepartamentoResponseDTO>> GetAllAsync();
        Task<DepartamentoResponseDTO> GetByIdAsync(int idDepartamento);
        Task<DepartamentoResponseDTO> AddAsync(DepartamentoCreateDTO dto);
        Task<bool> UpdateAsync(int idDepartamento, DepartamentoUpdateDTO dto);
        Task<bool> DeleteAsync(int idDepartamento);
    }
}

