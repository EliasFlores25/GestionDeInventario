using GestionDeInventario.DTOs.UsuarioDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;

namespace GestionDeInventario.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepo;

        private const string ROL_POR_DEFECTO = "Administrador";
        public AuthService(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        // --- REGISTRO ---
        public async Task<UsuarioResponseDTO> RegistrarAsync(UsuarioRegisterDTO dto)
        {
            // 1. Validar si el email ya existe (Lógica de Negocio)
            if (await _usuarioRepo.GetByEmailAsync(dto.email) != null)
            {
                throw new ConflictException("El correo ya está registrado.");
            }
            // 2. Mapeo y HASHING (Lógica de Seguridad)
            var usuario = new Usuario
            {
                nombre = dto.nombre,
                email = dto.email,
                contraseña = BCrypt.Net.BCrypt.HashPassword(dto.contraseña),
                tipoRol = ROL_POR_DEFECTO
            };

            // 3. Guardar en DB (Delegación al Repositorio)
            var usuarioGuardado = await _usuarioRepo.AddAsync(usuario);

            return new UsuarioResponseDTO
            {
                idUsuario = usuarioGuardado.idUsuario,
                nombre = usuarioGuardado.nombre,
                email = usuarioGuardado.email,
                tipoRol = usuarioGuardado.tipoRol
            };
        }
        // --- LOGIN ---
        public async Task<UsuarioResponseDTO?> LoginAsync(UsuarioLoginDTO dto)
        {
            // 1. Buscar Usuario (Delegación al Repositorio)
            var usuario = await _usuarioRepo.GetByEmailAsync(dto.email);
            if (usuario == null) return null; // Email no encontrado

            // 2. Verificar Contraseña (Lógica de Seguridad)
            if (!BCrypt.Net.BCrypt.Verify(dto.contraseña, usuario.contraseña)) // ⬅️ Verificación
            {
                return null; // Contraseña incorrecta
            }

            return new UsuarioResponseDTO
            {
                idUsuario = usuario.idUsuario,
                nombre = usuario.nombre,
                email = usuario.email,
               tipoRol = usuario.tipoRol
            };
        }
    }
}
