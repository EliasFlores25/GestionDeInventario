using GestionDeInventario.DTOs.UsuarioDTOs;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; 

namespace GestionDeInventario.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        // === ACCIÓN DE LOGIN (GET) ===
        // Muestra el formulario de inicio de sesión
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(); // Devuelve Views/Auth/Login.cshtml
        }
        // === ACCIÓN DE LOGIN (POST) ===
        // Procesa el formulario y establece la autenticación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UsuarioLoginDTO dto, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(dto);
            }
            // 1. Llama al Servicio para validar credenciales (Hashing/Verify)
            var usuarioDto = await _authService.LoginAsync(dto);

            if (usuarioDto == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                ViewData["ReturnUrl"] = returnUrl;
                return View(dto);
            }
            // 2. Crear Claims (Identidad del Usuario)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioDto.idUsuario.ToString()),
                new Claim(ClaimTypes.Email, usuarioDto.email),
                new Claim(ClaimTypes.Role, usuarioDto.tipoRol.ToString()), // El rol es vital para la autorización
                new Claim(ClaimTypes.Name, usuarioDto.nombre)
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. Establecer la Sesión (Cookie)
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity));

            // 4. Redirigir al usuario
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home"); // O a la página principal del inventario
        }
        // === ACCIÓN DE LOGOUT ===
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
        // === ACCIÓN DE REGISTRAR (GET) ===
        // Muestra el formulario de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View(); // Devuelve Views/Auth/Register.cshtml
        }
        // === ACCIÓN DE REGISTRAR (POST) ===
        // Procesa el formulario de registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UsuarioRegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                await _authService.RegistrarAsync(dto);

                // 🔑 Flujo de Éxito: Redirección al Login
                TempData["SuccessMessage"] = "¡Registro exitoso! Ya puedes iniciar sesión.";
                return RedirectToAction("Login");

            }
            catch (ConflictException ex)
            {
                // ❌ Flujo de Error Conocido (Email duplicado): Vuelve a la vista con error
                ModelState.AddModelError(nameof(dto.email), ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                // ❌ Flujo de Error Desconocido (DB falló, etc.): Vuelve a la vista con un mensaje general
                ModelState.AddModelError(string.Empty, $"Ocurrió un error inesperado durante el registro: {ex.Message}");
                return View(dto);
            }
        }
    }
}