using GestionDeInventario.DTOs.ProductoDTOs;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }
        public async Task<IActionResult> Index(string nombre, string estado, int pageNumber = 1, int pageSize = 5)
        {
            IQueryable<ProductoResponseDTO> query = _productoService.GetQueryable();
            string? n_nombre = nombre?.ToLower();
            string? n_estado = estado?.ToLower();

            if (!string.IsNullOrWhiteSpace(n_nombre))
            {
                query = query.Where(c => c.nombre.ToLower().Contains(n_nombre));
            }
            if (!string.IsNullOrWhiteSpace(n_estado))
            {
                query = query.Where(c => c.estado.ToLower().Contains(n_estado));
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
                ViewBag.CurrentApellidoEmpleado = estado;
                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;

                if (!string.IsNullOrWhiteSpace(nombre) || !string.IsNullOrWhiteSpace(estado))
                {
                    ViewData["IsFilterApplied"] = true;
                }
                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al cargar la lista de productos: " + ex.Message;
                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = 0;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;

                return View(new List<ProductoResponseDTO>());
            }
        }

        public IActionResult Create()
        {
            return View(new ProductoCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                var nuevoProducto = await _productoService.AddAsync(dto);
                if (nuevoProducto == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el empleado.");
                    return View(dto);
                }
                TempData["Ok"] = "Producto creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el empleado: " + ex.Message);
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var productoDto = await _productoService.GetByIdAsync(id);
                if (productoDto == null)
                {
                    return NotFound();
                }
                var updateDto = new ProductoUpdateDTO
                {
                    nombre = productoDto.nombre,
                    descripcion = productoDto.descripcion,
                    cantidadStock = productoDto.cantidadStock,
                    unidadMedida = productoDto.unidadMedida,
                    precio = productoDto.precio,
                    estado = productoDto.estado
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
                TempData["ErrorMessage"] = "No se pudo cargar el producto para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try
            {
                var success = await _productoService.UpdateAsync(id, dto);
                if (success)
                {
                    TempData["Ok"] = "Producto actualizado con éxito.";
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
                ModelState.AddModelError("", "Error al actualizar el producto. Verifique si el ID coincide o si la sesión es válida.");
            }
            return View(dto);
        }

        public async Task<IActionResult> Details(int id)
        {
            var producto = await _productoService.GetByIdAsync(id);

            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var producto = await _productoService.GetByIdAsync(id);
                return View(producto);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El producto solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productoService.DeleteAsync(id);
                TempData["MensajeExito"] = "Producto eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El producto ya no existe o fue eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el producto: " + ex.Message);
                try
                {
                    var producto = await _productoService.GetByIdAsync(id);
                    return View("Delete", producto);
                }
                catch (NotFoundException)
                {
                    TempData["MensajeError"] = "Error interno: El producto fue eliminado antes de mostrar el error.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }
    }
}
