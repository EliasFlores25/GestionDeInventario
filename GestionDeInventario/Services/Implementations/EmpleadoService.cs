using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        public EmpleadoService(IEmpleadoRepository empleadoRepository)
        {
            _empleadoRepository = empleadoRepository;
        }
        public IQueryable<Empleado> GetQueryable()
        {
            return _empleadoRepository.GetQueryable();
        }
        public async Task<List<Empleado>> GetAllAsync() =>
    (await _empleadoRepository.GetAllAsync()).Select(x => new Empleado
    {
        idEmpleado = x.idEmpleado,
        nombre = x.nombre,
        apellido = x.apellido,
        edad = x.edad,
        genero = x.genero,
        telefono = x.telefono,
        direccion = x.direccion,
        departamentoId = x.departamentoId,
        estado = x.estado,
        }).ToList();

        public async Task<Empleado?> GetByIdAsync(int id)
        {
            var x = await _empleadoRepository.GetByIdAsync(id);
            return x == null ? null : new Empleado
            {
                idEmpleado = x.idEmpleado,
                nombre = x.nombre,
                apellido = x.apellido,
                edad = x.edad,
                genero = x.genero,
                telefono = x.telefono,
                direccion = x.direccion,
                departamentoId = x.departamentoId,
                estado = x.estado,
                };
        }
        public async Task<Empleado> AddAsync(Empleado categoria)
        {
            var entity = new Empleado
            {
                nombre = categoria.nombre.Trim(),
                apellido = categoria.apellido.Trim(),
                edad = categoria.edad,
                genero = categoria.genero.Trim(),
                telefono = categoria.telefono.Trim(),
                direccion = categoria.direccion.Trim(),
                departamentoId = categoria.departamentoId,
                estado = categoria.estado.Trim()
                };
            var saved = await _empleadoRepository.AddAsync(entity);
            return new Empleado
            {
                idEmpleado = saved.idEmpleado,
                nombre = saved.nombre,
                apellido = saved.apellido,
                edad = saved.edad,
                genero = saved.genero,
                telefono = saved.telefono,
                direccion = saved.direccion,
                departamentoId = saved.departamentoId,
                estado = saved.estado
            };
        }
        public async Task<bool> UpdateAsync(int id, Empleado empleado)
        {
            var current = await _empleadoRepository.GetByIdAsync(id);
            if (current == null) return false;
            current.nombre = empleado.nombre.Trim();
            current.apellido = empleado.apellido.Trim();
            current.edad = empleado.edad;
            current.genero = empleado.genero.Trim();
            current.telefono = empleado.telefono.Trim();
            current.direccion = empleado.direccion.Trim();
            current.departamentoId = empleado.departamentoId;
            current.estado = empleado.estado.Trim();
            return await _empleadoRepository.UpdateAsync(current);
        }
        public Task<bool> DeleteAsync(int id) => _empleadoRepository.DeleteAsync(id);
    }
}
