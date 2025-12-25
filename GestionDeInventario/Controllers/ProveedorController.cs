using GestionDeInventario.DTOs.ProveedorDTOs;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProveedorController : Controller
    {
        private readonly IProveedorService _proveedorService;

        public ProveedorController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }
        public async Task<IActionResult> Index(string nombreEmpresa, string estado, int pageNumber = 1, int pageSize = 5)
        {
            IQueryable<ProveedorResponseDTO> query = _proveedorService.GetQueryable();
            string? n_nombreEmpresa = nombreEmpresa?.ToLower();
            string? n_estado = estado?.ToLower();
            if (!string.IsNullOrWhiteSpace(n_nombreEmpresa)) query = query.Where(c => c.nombreEmpresa.ToLower().Contains(n_nombreEmpresa));
            if (!string.IsNullOrWhiteSpace(n_estado)) query = query.Where(c => c.estado.ToLower().Contains(n_estado));
            try
            {
                int totalRegistros = await query.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));
                var listaPaginada = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                ViewBag.CurrentNombreEmpleado = nombreEmpresa;
                ViewBag.CurrentApellidoEmpleado = estado;
                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;
                if (!string.IsNullOrWhiteSpace(nombreEmpresa) || !string.IsNullOrWhiteSpace(estado)) ViewData["IsFilterApplied"] = true;
                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al cargar la lista de proveedor: " + ex.Message;
                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = 0;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;
                return View(new List<ProveedorResponseDTO>());
            }
        }
        public IActionResult Create()
        {
            return View(new ProveedorCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProveedorCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                var nuevoProveedor = await _proveedorService.AddAsync(dto);
                if (nuevoProveedor == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el proveedor.");
                    return View(dto);
                }
                TempData["Ok"] = "Proveedor creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el proveedor: " + ex.Message);
                return View(dto);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var proveedorDto = await _proveedorService.GetByIdAsync(id);
                if (proveedorDto == null)
                {
                    return NotFound();
                }
                var updateDto = new ProveedorUpdateDTO
                {
                    nombreEmpresa = proveedorDto.nombreEmpresa,
                    direccion = proveedorDto.direccion,
                    telefono = proveedorDto.telefono,
                    email = proveedorDto.email,
                    estado = proveedorDto.estado
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
                TempData["ErrorMessage"] = "No se pudo cargar el proveedor para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProveedorUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                var success = await _proveedorService.UpdateAsync(id, dto);
                if (success)
                {
                    TempData["Ok"] = "Proveedor actualizado con éxito.";
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
                ModelState.AddModelError("", "Error al actualizar el proveedor. Verifique si el ID coincide o si la sesión es válida.");
            }
            return View(dto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var proveedor = await _proveedorService.GetByIdAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }
            return View(proveedor);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var proveedor = await _proveedorService.GetByIdAsync(id);
                return View(proveedor);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El proveedor solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _proveedorService.DeleteAsync(id);
                TempData["MensajeExito"] = "Proveedor eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El proveedor ya no existe o fue eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el proveedor: " + ex.Message);
                try
                {
                    var proveedor = await _proveedorService.GetByIdAsync(id);
                    return View("Delete", proveedor);
                }
                catch (NotFoundException)
                {
                    TempData["MensajeError"] = "Error interno: El proveedor fue eliminado antes de mostrar el error.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }
    }
}