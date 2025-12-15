using GestionDeInventario.DTOs.UsuarioDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepo;
        public UsuarioService(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }
        public IQueryable<UsuarioResponseDTO> GetQueryable()
        {
            return _usuarioRepo.GetQueryable()
                .Select(u => new UsuarioResponseDTO 
                {
                    idUsuario = u.idUsuario,
                    nombre = u.nombre,
                    email = u.email,
                    tipoRol = u.tipoRol,
                });
        }
        private UsuarioResponseDTO MapToResponseDTO(Usuario u)
        {
            return new UsuarioResponseDTO
            {
                idUsuario = u.idUsuario,
                nombre = u.nombre,
                email = u.email,
                tipoRol = u.tipoRol
            };
        }
        public async Task<List<UsuarioResponseDTO>> GetAllAsync() =>
            ( await _usuarioRepo.GetAllAsync()).Select(MapToResponseDTO).ToList();
        public async Task<UsuarioResponseDTO> GetByEmailAsync(string email)
        {
            var usuario = await _usuarioRepo.GetByEmailAsync(email);

            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuario con email {email} no encontrado.");
            }
            return MapToResponseDTO(usuario);
        }
        public async Task<UsuarioResponseDTO> GetByIdAsync(int idUsuario)
        {
            var usuario = await _usuarioRepo.GetByIdAsync(idUsuario);
            if (usuario == null)
            {
                throw new NotFoundException($"Usuario con ID {idUsuario} no encontrado.");
            }
            return MapToResponseDTO(usuario);
        }
        public async Task<UsuarioResponseDTO> AddAsync(UsuarioRegisterDTO dto)
        {
            if (await _usuarioRepo.GetByEmailAsync(dto.email) != null)
            {
                throw new ConflictException($"El email '{dto.email}' ya existe.");
            }
            var nuevoUsuario = new Usuario
            {
                nombre = dto.nombre,
                email = dto.email,
                tipoRol = "Administrador",
                contraseña = "CONTRASEÑA_TEMPORAL"
            };
            var usuarioGuardado = await _usuarioRepo.AddAsync(nuevoUsuario);
            return MapToResponseDTO(usuarioGuardado);
        }
        public async Task<bool> UpdateAsync(int idUsuario, UsuarioUpdateDTO dto)
        {
            var usuarioExistente = await _usuarioRepo.GetByIdAsync(idUsuario);
            if (usuarioExistente == null)
            {
                throw new NotFoundException($"Usuario con ID {idUsuario} no encontrado para la actualización.");
            }
            usuarioExistente.nombre = dto.nombre ?? usuarioExistente.nombre;
            usuarioExistente.email = dto.email ?? usuarioExistente.email;
            usuarioExistente.tipoRol = dto.tipoRol;
            // ⚠️ NO se debe actualizar el ContraseniaHash aquí a menos que sea un cambio de contraseña

            return await _usuarioRepo.UpdateAsync(usuarioExistente);
        }
        public async Task<bool> DeleteAsync(int idUsuario)
        {
            bool wasDeleted = await _usuarioRepo.DeleteAsync(idUsuario);
            if (!wasDeleted)
            {
                throw new NotFoundException($"Usuario con ID  {idUsuario} no existe para ser eliminado.");
            }
            return true;
        }
    }   
}