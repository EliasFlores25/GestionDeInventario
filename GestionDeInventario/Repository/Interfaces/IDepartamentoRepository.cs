using GestionDeInventario.Models;

namespace GestionDeInventario.Repository.Interfaces
{
    public interface IDepartamentoService
    {
        Task<IEnumerable<Departamento>> GetAll();
        Task<Departamento> GetById(int id);
        Task Add(Departamento departamento);
        Task Update(Departamento departamento);
        Task Delete(int id);
    }
}