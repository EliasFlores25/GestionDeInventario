using GestionDeInventario.DTOs.DepartamentoDTOs;
using GestionDeInventario.DTOs.ProductoDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    public class DepartamentoController : Controller
    {
        private readonly IDepartamentoService _departamentoService;

        public DepartamentoController(IDepartamentoService departamentoService)
        {
            _departamentoService = departamentoService;
        }

        public async Task<IActionResult> Index(string nombre, int pageNumber = 1, int pageSize = 5)
        {
            IQueryable<DepartamentoResponseDTO> query = _departamentoService.GetQueryable();
            string? n_nombre = nombre?.ToLower();

            if (!string.IsNullOrWhiteSpace(n_nombre))
            {
                query = query.Where(c => c.nombre.ToLower().Contains(n_nombre));
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
                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    ViewData["IsFilterApplied"] = true;
                }
                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al cargar la lista de departamentos: " + ex.Message;
                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = 0;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;

                return View(new List<DepartamentoResponseDTO>());
            }
        }


        public IActionResult Create()
        {
            return View(new DepartamentoCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartamentoCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                var nuevoDepartamento = await _departamentoService.AddAsync(dto);
                if (nuevoDepartamento == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el departamento.");
                    return View(dto);
                }
                TempData["Ok"] = "Departamento creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el departamento: " + ex.Message);
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var departamentoDto = await _departamentoService.GetByIdAsync(id);
                if (departamentoDto == null)
                {
                    return NotFound();
                }
                var updateDto = new DepartamentoUpdateDTO
                {
                    nombre = departamentoDto.nombre,
                    descripcion = departamentoDto.descripcion,
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
                TempData["ErrorMessage"] = "No se pudo cargar el departamento para edición.";
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartamentoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                var success = await _departamentoService.UpdateAsync(id, dto);
                if (success)
                {
                    TempData["Ok"] = "Departamento actualizado con éxito.";
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
                ModelState.AddModelError("", "Error al actualizar el departamento. Verifique si el ID coincide o si la sesión es válida.");
            }
            return View(dto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var departamento = await _departamentoService.GetByIdAsync(id);

            if (departamento == null)
            {
                return NotFound();
            }
            return View(departamento);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var departamento = await _departamentoService.GetByIdAsync(id);
                return View(departamento);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El departamento solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _departamentoService.DeleteAsync(id);
                TempData["MensajeExito"] = "Departamento eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El departamento ya no existe o fue eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el departamento: " + ex.Message);
                try
                {
                    var departamento = await _departamentoService.GetByIdAsync(id);
                    return View("Delete", departamento);
                }
                catch (NotFoundException)
                {
                    TempData["MensajeError"] = "Error interno: El departamento fue eliminado antes de mostrar el error.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }
    }
}
