using GestionDeInventario.DTOs.DepartamentoDTOs;
using GestionDeInventario.DTOs.ProductoDTOs;
using GestionDeInventario.Models;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Implementations;
using GestionDeInventario.Services.Interfaces;
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

        public async Task<IActionResult> Index(string nombre, int registros = 0)
        {

            IQueryable<DepartamentoResponseDTO> query = _departamentoService.GetQueryable();

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(c => c.nombre.ToLower().Contains(nombre.ToLower()));
            }
            if (registros > 0)
            {
                query = query.Take(registros);
            }
            try
            {
                List<DepartamentoResponseDTO> listaFiltrada = await query.ToListAsync();

                ViewData["CurrentFilter"] = nombre;
                ViewData["CurrentRecords"] = registros;

                return View(listaFiltrada);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Ocurrió un error al cargar la lista de departamentos: " + ex.Message;

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
                await _departamentoService.AddAsync(dto);

                TempData["SuccessMessage"] = "Departamento creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al crear el departamento. Inténtelo de nuevo.";

                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var departamentoDto = await _departamentoService.GetByIdAsync(id);
                var updateDto = new DepartamentoUpdateDTO
                {
                    nombre = departamentoDto.nombre,
                    descripcion = departamentoDto.descripcion,
                };
                ViewBag.id = id;
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
                ViewBag.id = id;
                return View(dto);
            }
            try
            {
                await _departamentoService.UpdateAsync(id, dto);

                TempData["SuccessMessage"] = "Departamento actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.id = id;
                return View(dto);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                DepartamentoResponseDTO departamentoResponseDTO = await _departamentoService.GetByIdAsync(id);

                return View(departamentoResponseDTO);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var departamento = await _departamentoService.GetByIdAsync(id.Value);

            if (departamento == null)
            {
                TempData["MensajeError"] = "Error: El departamento solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View(departamento);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _departamentoService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Departamento eliminado exitosamente.";
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al intentar eliminar el departamento.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
