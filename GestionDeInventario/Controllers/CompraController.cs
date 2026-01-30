using GestionDeInventario.DTOs.CompraDTOs;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CompraController : Controller
    {
        private readonly ICompraService _compraService;
        private readonly IUsuarioService _usuarioService;
        private readonly IProveedorService _proveedorService;

        public CompraController(ICompraService compraService, IUsuarioService usuarioService, IProveedorService proveedorService)
        {
            _compraService = compraService;
            _usuarioService = usuarioService;
            _proveedorService = proveedorService;
        }

        private async Task PopulateDropdowns()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            var proveedores = await _proveedorService.GetAllAsync();

            ViewBag.UsuarioId = new SelectList(usuarios, "idUsuario", "nombre");
            ViewBag.ProveedorId = new SelectList(proveedores, "idProveedor", "nombreEmpresa");
        }

        private async Task PopulateFilterDataViewBag()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            var proveedores = await _proveedorService.GetAllAsync();

            // Listas para los dropdowns de búsqueda
            var usuariosList = usuarios.Select(u => new SelectListItem
            {
                Value = u.idUsuario.ToString(),
                Text = u.nombre
            }).ToList();
            usuariosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Usuarios" });
            ViewBag.UsuariosFiltro = usuariosList;

            var proveedoresList = proveedores.Select(p => new SelectListItem
            {
                Value = p.idProveedor.ToString(),
                Text = p.nombreEmpresa
            }).ToList();
            proveedoresList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Proveedores" });
            ViewBag.ProveedoresFiltro = proveedoresList;

            // Diccionarios para mostrar nombres en el Index
            ViewBag.UsuariosNombres = usuarios.ToDictionary(u => u.idUsuario, u => u.nombre);
            ViewBag.ProveedoresNombres = proveedores.ToDictionary(p => p.idProveedor, p => p.nombreEmpresa);
        }

        public async Task<IActionResult> Index(string numeroFactura, int? usuarioId, int? proveedorId, int pageNumber = 1, int pageSize = 5)
        {
            await PopulateFilterDataViewBag();
            IQueryable<CompraResponseDTO> query = _compraService.GetQueryable();

            // Lógica de Filtrado
            if (!string.IsNullOrWhiteSpace(numeroFactura))
            {
                query = query.Where(c => c.NumeroFactura.ToLower().Contains(numeroFactura.ToLower()));
            }

            if (usuarioId.HasValue && usuarioId.Value > 0)
            {
                query = query.Where(c => c.UsuarioId == usuarioId.Value);
            }

            if (proveedorId.HasValue && proveedorId.Value > 0)
            {
                query = query.Where(c => c.ProveedorId == proveedorId.Value);
            }

            try
            {
                int totalRegistros = await query.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);

                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

                var listaPaginada = await query
                    .OrderByDescending(c => c.FechaCompra) // Opcional: ordenar por fecha
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Mantener filtros en la vista
                ViewBag.CurrentNumeroFactura = numeroFactura;
                ViewBag.CurrentUsuarioId = usuarioId;
                ViewBag.CurrentProveedorId = proveedorId;

                // Datos de paginación
                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;

                if (!string.IsNullOrWhiteSpace(numeroFactura) || usuarioId.HasValue || proveedorId.HasValue)
                {
                    ViewData["IsFilterApplied"] = true;
                }

                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al cargar la lista de compras: " + ex.Message;
                await PopulateFilterDataViewBag();

                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = 0;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;

                return View(new List<CompraResponseDTO>());
            }
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new CompraCreateDTO { FechaCompra = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompraCreateDTO compraDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(compraDto);
            }
            try
            {
                var nuevaCompra = await _compraService.AddAsync(compraDto);
                if (nuevaCompra == null)
                {
                    ModelState.AddModelError("", "No se pudo registrar la compra.");
                    await PopulateDropdowns();
                    return View(compraDto);
                }
                TempData["Ok"] = "Compra registrada con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException brex)
            {
                ModelState.AddModelError(string.Empty, brex.Message);
                await PopulateDropdowns();
                return View(compraDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear la compra: " + ex.Message);
                await PopulateDropdowns();
                return View(compraDto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var compraDto = await _compraService.GetByIdAsync(id);
                if (compraDto == null) return NotFound();

                var updateDto = new CompraUpdateDTO
                {
                    IdCompra = compraDto.IdCompra,
                    NumeroFactura = compraDto.NumeroFactura,
                    UsuarioId = compraDto.UsuarioId,
                    ProveedorId = compraDto.ProveedorId,
                    FechaCompra = compraDto.FechaCompra
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
                TempData["ErrorMessage"] = "No se pudo cargar la compra para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompraUpdateDTO compra)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(compra);
            }
            try
            {
                var success = await _compraService.UpdateAsync(id, compra);
                if (success)
                {
                    TempData["Ok"] = "Compra actualizada con éxito.";
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
                ModelState.AddModelError("", "Error al actualizar la compra.");
            }
            await PopulateDropdowns();
            return View(compra);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var compra = await _compraService.GetByIdAsync(id);
                if (compra == null) return NotFound();

                await PopulateFilterDataViewBag(); // Para obtener los diccionarios de nombres
                return View(compra);
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var compra = await _compraService.GetByIdAsync(id);
                await PopulateFilterDataViewBag();
                return View(compra);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: La compra solicitada no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _compraService.DeleteAsync(id);
                TempData["Ok"] = "Compra eliminada con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["Error"] = "Error: La compra ya no existe.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar la compra: " + ex.Message);
                var compra = await _compraService.GetByIdAsync(id);
                await PopulateFilterDataViewBag();
                return View("Delete", compra);
            }
        }
    }
}