using GestionDeInventario.DTOs.UsuarioDTOs;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        public async Task<IActionResult> Index(string nombre, string tipoRol, int pageNumber = 1, int pageSize = 5)
        {
            IQueryable<UsuarioResponseDTO> query = _usuarioService.GetQueryable();
            string? n_nombre = nombre?.ToLower();
            string? n_estado = tipoRol?.ToLower();

            if (!string.IsNullOrWhiteSpace(n_nombre))
            {
                query = query.Where(c => c.nombre.ToLower().Contains(n_nombre));
            }
            if (!string.IsNullOrWhiteSpace(n_estado))
            {
                query = query.Where(c => c.tipoRol.ToLower().Contains(n_estado));
            }
            try
            {
                int totalRegistros = await query.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));
                var listaPaginada = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                ViewBag.CurrentNombreEmpleado = nombre;
                ViewBag.CurrentApellidoEmpleado = tipoRol;
                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;
                if (!string.IsNullOrWhiteSpace(nombre) || !string.IsNullOrWhiteSpace(tipoRol))
                {
                    ViewData["IsFilterApplied"] = true;
                }
                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al cargar la lista de usuarios: " + ex.Message;
                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = 0;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;

                return View(new List<UsuarioResponseDTO>());
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var usuarioDto = await _usuarioService.GetByIdAsync(id);
                if (usuarioDto == null)
                {
                    return NotFound();
                }
                var updateDto = new UsuarioUpdateDTO
                {
                    nombre = usuarioDto.nombre,
                    tipoRol = usuarioDto.tipoRol,
                    email = usuarioDto.email,
                    contraseña = usuarioDto.contraseña,
                };
                return View(updateDto);
            }

            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el usuario para edición.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                var success = await _usuarioService.UpdateAsync(id, dto);
                if (success)
                {
                    TempData["Ok"] = "Usuario actualizado con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "La actualización no se pudo completar. El registro no existe o el servicio falló.");
                }
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al actualizar el usuario. Verifique si el ID coincide o si la sesión es válida.");
            }
            return View(dto);
        }
        public async Task<IActionResult> Details(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetByIdAsync(id);
                return View(usuario);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El usuario solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _usuarioService.DeleteAsync(id);
                TempData["MensajeExito"] = "Usuario eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El usuario ya no existe o fue eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el usuario: " + ex.Message);
                try
                {
                    var usuario = await _usuarioService.GetByIdAsync(id);
                    return View("Delete", usuario);
                }
                catch (NotFoundException)
                {
                    TempData["MensajeError"] = "Error interno: El usuario fue eliminado antes de mostrar el error.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }
    }
}