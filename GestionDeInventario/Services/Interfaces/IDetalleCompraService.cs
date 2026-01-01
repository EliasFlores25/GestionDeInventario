using GestionDeInventario.DTOs.DetalleCompraDTOs;

namespace GestionDeInventario.Services.Interfaces
{
    public interface IDetalleCompraService
    {
        IQueryable<DetalleCompraResponseDTO> GetQueryable();
        Task<List<DetalleCompraResponseDTO>> GetAllAsync();
        Task<DetalleCompraResponseDTO> GetByIdAsync(int idDetalleCompra);
        Task<DetalleCompraResponseDTO> AddAsync(DetalleCompraCreateDTO dto);
        Task<bool> UpdateAsync(int idDetalleCompra, DetalleCompraUpdateDTO dto);
        Task<bool> DeleteAsync(int idDetalleCompra);

        IQueryable<DetalleCompraExcelDTO> GetQueryableForExcel();
    }
}