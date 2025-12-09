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

        public async Task<IActionResult> Index(string nombre,int? cantidadStock, int registros = 0)
        {
            
            IQueryable<ProductoResponseDTO> query = _productoService.GetQueryable();

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(c => c.nombre.ToLower().Contains(nombre.ToLower()));
            }
            if (cantidadStock.HasValue)
            {
                query = query.Where(c => c.cantidadStock == cantidadStock.Value);
            }
            if (registros > 0)
            {
                query = query.Take(registros);
            }

            try
            {
                List<ProductoResponseDTO> listaFiltrada = await query.ToListAsync();

                ViewData["NombreFilter"] = nombre;
                ViewData["StockFilter"] = cantidadStock;
                ViewData["CurrentRecords"] = registros;

                return View(listaFiltrada);
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Ocurrió un error al cargar la lista de productos.";
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
                await _productoService.AddAsync(dto);

                TempData["SuccessMessage"] = "Producto creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al crear el producto. Inténtelo de nuevo.";
               
                return View(dto);
            }
        }
       
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
              
                var productoDto = await _productoService.GetByIdAsync(id);

                var updateDto = new ProductoUpdateDTO
                {
                    nombre = productoDto.nombre,
                    descripcion = productoDto.descripcion,
                    cantidadStock = productoDto.cantidadStock,
                    unidadMedida = productoDto.unidadMedida,
                    precio = productoDto.precio,
                    estado = productoDto.estado
                };

                ViewBag.IdProducto = id;
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
                ViewBag.IdProducto = id;
                return View(dto);
            }

            try
            {
                await _productoService.UpdateAsync(id, dto);

                TempData["SuccessMessage"] = "Producto actualizado exitosamente.";
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
                ViewBag.IdProducto = id;
                return View(dto);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                ProductoResponseDTO producto = await _productoService.GetByIdAsync(id);

                return View(producto);
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
            var producto = await _productoService.GetByIdAsync(id.Value);

            if (producto == null)
            {
                TempData["MensajeError"] = "Error: El producto solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productoService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Producto eliminado exitosamente.";
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al intentar eliminar el producto.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
