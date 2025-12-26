using GestionDeInventario.DTOs.EmpleadoDTOs;
using GestionDeInventario.Repository.Interfaces;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;
using GestionDeInventario.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EmpleadoController : Controller
    {
        private readonly IEmpleadoService _empleadoService;
        private readonly IDepartamentoService _departamentoService;
        public EmpleadoController(IEmpleadoService empleadoService, IDepartamentoService departamentoService)
        {
            _empleadoService = empleadoService;
            _departamentoService = departamentoService;
        }
        private async Task PopulateDropdowns()
        {
            var departamentos = await _departamentoService.GetAllAsync();
            ViewBag.departamentoId = new SelectList(departamentos, "idDepartamento", "nombre");
        }
        private async Task PopulateDepartamentoNamesViewBag()
        {
            var departamentos = await _departamentoService.GetAllAsync();

            var departamentosList = departamentos.Select(d => new SelectListItem
            {
                Value = d.idDepartamento.ToString(),
                Text = d.nombre
            }).ToList();
            departamentosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Departamentos" });
            ViewBag.departamentoId = departamentosList;

            ViewBag.DepartamentosNombres = departamentos.ToDictionary(d => d.idDepartamento, d => d.nombre);

        }
        public async Task<IActionResult> Index(string nombre, string apellido, string genero, int? departamentoId, int pageNumber = 1, int pageSize = 5)
        {
            await PopulateDepartamentoNamesViewBag();
            IQueryable<EmpleadoResponseDTO> query = _empleadoService.GetQueryable();

            string? n_nombre = nombre?.ToLower();
            string? n_apellido = apellido?.ToLower();
            string? n_genero = genero?.ToLower();

            if (!string.IsNullOrWhiteSpace(n_nombre))
            {
                query = query.Where(c => c.nombre.ToLower().Contains(n_nombre));
            }

            if (!string.IsNullOrWhiteSpace(n_apellido))
            {
                query = query.Where(c => c.apellido.ToLower().Contains(n_apellido));
            }

            if (!string.IsNullOrWhiteSpace(n_genero))
            {
                query = query.Where(c => c.genero.ToLower() == n_genero);
            }

            if (departamentoId.HasValue && departamentoId.Value > 0)
            {
                query = query.Where(c => c.departamentoId == departamentoId.Value);
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
                ViewBag.CurrentApellidoEmpleado = apellido;
                ViewBag.CurrentGenero = genero;
                ViewBag.CurrentDepartamentoId = departamentoId;

                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;

                if (!string.IsNullOrWhiteSpace(nombre) || !string.IsNullOrWhiteSpace(apellido) || !string.IsNullOrWhiteSpace(genero) || departamentoId.HasValue && departamentoId.Value > 0)
                {
                    ViewData["IsFilterApplied"] = true;
                }
                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al cargar la lista de empleados: " + ex.Message;
                await PopulateDepartamentoNamesViewBag();

                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = 0;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;

                return View(new List<EmpleadoResponseDTO>());
            }
        }
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new EmpleadoCreateDTO());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpleadoCreateDTO empleadoDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(empleadoDto);
            }
            try
            {
                var nuevoEmpleado = await _empleadoService.AddAsync(empleadoDto);
                if (nuevoEmpleado == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el empleado.");
                    await PopulateDropdowns();
                    return View(empleadoDto);
                }
                TempData["Ok"] = "Empleado creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException brex)
            {
                ModelState.AddModelError(string.Empty, brex.Message);
                await PopulateDropdowns();
                return View(empleadoDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el empleado: " + ex.Message);
                await PopulateDropdowns();
                return View(empleadoDto);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var empleadoDto = await _empleadoService.GetByIdAsync(id);
                if (empleadoDto == null)
                {
                    return NotFound();
                }
                var updateDto = new EmpleadoUpdateDTO
                {
                    nombre = empleadoDto.nombre,
                    apellido = empleadoDto.apellido,
                    edad = empleadoDto.edad,
                    genero = empleadoDto.genero,
                    telefono = empleadoDto.telefono,
                    direccion = empleadoDto.direccion,
                    departamentoId = empleadoDto.departamentoId,
                    estado = empleadoDto.estado,
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
                TempData["ErrorMessage"] = "No se pudo cargar el empleado para edición.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmpleadoUpdateDTO empleado)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(empleado);
            }
            try
            {
                var success = await _empleadoService.UpdateAsync(id, empleado);
                if (success)
                {
                    TempData["Ok"] = "Empleado actualizado con éxito.";
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
                ModelState.AddModelError("", "Error al actualizar el empleado. Verifique si el ID coincide o si la sesión es válida.");
            }
            await PopulateDropdowns();
            return View(empleado);
        }
        public async Task<IActionResult> Details(int id)
        {
            var empleado = await _empleadoService.GetByIdAsync(id);
            var departamentos = await _departamentoService.GetAllAsync();

            ViewBag.DepartamentoNombres = departamentos?.ToDictionary(d => d.idDepartamento, d => d.nombre) ?? new Dictionary<int, string>();
            if (empleado == null)
            {
                return NotFound();
            }
            return View(empleado);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var empleado = await _empleadoService.GetByIdAsync(id);
                var departamento = await _departamentoService.GetAllAsync();
                ViewBag.DepartamentoNombres = departamento?.ToDictionary(d => d.idDepartamento, d => d.nombre) ?? new Dictionary<int, string>();
                return View(empleado);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El empleado solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _empleadoService.DeleteAsync(id);
                TempData["MensajeExito"] = "Empleado eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El empleado ya no existe o fue eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el empleado: " + ex.Message);

                try
                {
                    var empleado = await _empleadoService.GetByIdAsync(id);
                    var departamento = await _departamentoService.GetAllAsync();
                    ViewBag.DepartamentoNombres = departamento?.ToDictionary(d => d.idDepartamento, d => d.nombre) ?? new Dictionary<int, string>();
                    return View("Delete", empleado);
                }
                catch (NotFoundException)
                {
                    TempData["MensajeError"] = "Error interno: El empleado fue eliminado antes de mostrar el error.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }
             // 1. Acción normal que dirige a la vista de la página web
        public IActionResult Perfil(int id)
        {
            var empleado = _empleadoService.GetByIdAsync(id);
            return View(empleado); // Esto busca Views/Empleado/Perfil.cshtml
        }

        // 2. Acción que genera y devuelve el PDF
        public async Task<IActionResult> DescargarPdf(int id)
        {
            // Buscamos los datos
            EmpleadoResponseDTO empleado = await _empleadoService.GetByIdAsync(id);

            // Usamos el módulo
            var pdfModule = new EmpleadoPDF();
            byte[] bytes = pdfModule.GenerarArchivoFicha(empleado);

            // Devolvemos el archivo directamente
            // "application/pdf" hace que el navegador lo abra o descargue
            return File(bytes, "application/pdf", $"Ficha_{empleado.apellido}.pdf");
        }
    }
}
