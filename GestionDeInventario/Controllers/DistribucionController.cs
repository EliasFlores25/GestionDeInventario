using GestionDeInventario.DTOs.CompraDTOs;
using GestionDeInventario.DTOs.DistribucionDTOs;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Implementations;
using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DistribucionController : Controller
    {
        private readonly IDistribucionService _distribucionService;
        private readonly IUsuarioService _usuarioService;
        private readonly IEmpleadoService _empleadoService;

        public DistribucionController(IDistribucionService distribucionService, IUsuarioService usuarioService, IEmpleadoService empleadoService)
        {
            _distribucionService = distribucionService;
            _usuarioService = usuarioService;
            _empleadoService = empleadoService;
        }

        private async Task PopulateDropdowns()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            var empleados = await _empleadoService.GetAllAsync();

            ViewBag.UsuarioId = new SelectList(usuarios, "idUsuario", "nombre");
            ViewBag.EmpleadoId = new SelectList(empleados, "idEmpleado", "nombre");
        }

        private async Task PopulateFilterDataViewBag()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            var empleados = await _empleadoService.GetAllAsync();

            var usuariosList = usuarios.Select(u => new SelectListItem
            {
                Value = u.idUsuario.ToString(),
                Text = u.nombre
            }).ToList();
            usuariosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Usuarios" });
            ViewBag.UsuarioId = usuariosList;

            var empleadosList = empleados.Select(e => new SelectListItem
            {
                Value = e.idEmpleado.ToString(),
                Text = e.nombre
            }).ToList();
            empleadosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Empleados" });
            ViewBag.EmpleadoId = empleadosList;

            ViewBag.UsuariosNombres = usuarios.ToDictionary(u => u.idUsuario, u => u.nombre);
            ViewBag.EmpleadosNombres = empleados.ToDictionary(e => e.idEmpleado, e => e.nombre);
        }

        public async Task<IActionResult> Index(string numeroDistribucion, int? usuarioId, int? empleadoId, int pageNumber = 1, int pageSize = 5)
        {
            await PopulateFilterDataViewBag();
            IQueryable<DistribucionResponseDTO> query = _distribucionService.GetQueryable();

            if (!string.IsNullOrWhiteSpace(numeroDistribucion))
            {
                query = query.Where(c => c.NumeroDistribucion.ToLower().Contains(numeroDistribucion.ToLower()));
            }

            if (usuarioId.HasValue && usuarioId.Value > 0)
            {
                query = query.Where(c => c.UsuarioId == usuarioId.Value);
            }

            if (empleadoId.HasValue && empleadoId.Value > 0)
            {
                query = query.Where(c => c.EmpleadoId == empleadoId.Value);
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

                ViewBag.CurrentNumeroDistribucion = numeroDistribucion;
                ViewBag.CurrentUsuarioId = usuarioId;
                ViewBag.CurrentEmpleadoId = empleadoId;

                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;

                if (!string.IsNullOrWhiteSpace(numeroDistribucion) || usuarioId.HasValue || empleadoId.HasValue)
                {
                    ViewData["IsFilterApplied"] = true;
                }

                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al cargar las distribuciones: " + ex.Message;
                await PopulateFilterDataViewBag();

                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = 0;
                return View(new List<DistribucionResponseDTO>());
            }
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new DistribucionCreateDTO { FechaSalida = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DistribucionCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(dto);
            }
            try
            {
                var resultado = await _distribucionService.AddAsync(dto);
                TempData["Ok"] = "Distribución creada con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException brex)
            {
                ModelState.AddModelError(string.Empty, brex.Message);
                await PopulateDropdowns();
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear: " + ex.Message);
                await PopulateDropdowns();
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dto = await _distribucionService.GetByIdAsync(id);
                if (dto == null) return NotFound();

                var updateDto = new DistribucionUpdateDTO
                {
                   IdDistribucion = dto.IdDistribucion,
                    NumeroDistribucion = dto.NumeroDistribucion,
                    UsuarioId = dto.UsuarioId,
                    EmpleadoId = dto.EmpleadoId,
                    FechaSalida = dto.FechaSalida,
                    Motivo = dto.Motivo
                };

                await PopulateDropdowns();
                return View(updateDto);
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "No se pudo cargar la distribución para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DistribucionUpdateDTO distribucion)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(distribucion);
            }
            try
            {
                var success = await _distribucionService.UpdateAsync(id, distribucion);
                if (success)
                {
                    TempData["Ok"] = "Distribucion actualizada con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "La actualización no se pudo completar.");
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
                ModelState.AddModelError("", "Error al actualizar la distribucion.");
            }
            await PopulateDropdowns();
            return View(distribucion);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var distribucion = await _distribucionService.GetByIdAsync(id);
                await PopulateFilterDataViewBag();
                return View(distribucion);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var distribucion = await _distribucionService.GetByIdAsync(id);
                await PopulateFilterDataViewBag();
                return View(distribucion);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "El registro no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _distribucionService.DeleteAsync(id);
                TempData["MensajeExito"] = "Distribución eliminada.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al eliminar: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}