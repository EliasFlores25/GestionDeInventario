using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IDepartamentoRepository _repository;

        public DepartamentoService(IDepartamentoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Departamento>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Departamento> GetById(int id)
        {
            return await _repository.GetById(id);
        }

        public async Task Add(Departamento departamento)
        {
            await _repository.Add(departamento);
        }

        public async Task Update(Departamento departamento)
        {
            await _repository.Update(departamento);
        }

        public async Task Delete(int id)
        {
            await _repository.Delete(id);
        }
    }
}
