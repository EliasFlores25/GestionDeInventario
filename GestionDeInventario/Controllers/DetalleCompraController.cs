using GestionDeInventario.DTOs.DetalleCompraDTOs;
using GestionDeInventario.Services.Interfaces;
using GestionDeInventario.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GestionDeInventario.Utilidades;

namespace GestionDeInventario.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DetalleCompraController : Controller
    {
        private readonly IDetalleCompraService _detalleCompraService;
        private readonly ICompraService _compraService;
        private readonly IProductoService _productoService;
        private readonly ILogger<DetalleCompraController> _logger;
        private readonly ExcelExporter _excelExporter;

        public DetalleCompraController(
            IDetalleCompraService detalleCompraService,
            ICompraService compraService,
            IProductoService productoService,
            ExcelExporter excelExporter,
            ILogger<DetalleCompraController> logger)
        {
            _detalleCompraService = detalleCompraService;
            _compraService = compraService;
            _productoService = productoService;
            _excelExporter = excelExporter;
            _logger = logger;
        }

        private async Task PopulateDropdowns()
        {
            var compras = await _compraService.GetAllAsync();
            var productos = await _productoService.GetAllAsync();

            ViewBag.CompraId = new SelectList(compras, "IdCompra", "NumeroFactura");
            ViewBag.ProductoId = new SelectList(productos, "idProducto", "nombre");
        }

        private async Task PopulateFilterDropdowns()
        {
            var compras = await _compraService.GetAllAsync();
            var productos = await _productoService.GetAllAsync();

            // Para filtros
            var comprasList = compras.Select(c => new SelectListItem
            {
                Value = c.IdCompra.ToString(),
                Text = c.NumeroFactura
            }).ToList();
            comprasList.Insert(0, new SelectListItem { Value = "", Text = "Todas las Compras" });
            ViewBag.CompraId = comprasList;

            var productosList = productos.Select(p => new SelectListItem
            {
                Value = p.idProducto.ToString(),
                Text = p.nombre
            }).ToList();
            productosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Productos" });
            ViewBag.ProductoId = productosList;

            // Para mostrar nombres en vistas
            ViewBag.ComprasNombres = compras.ToDictionary(c => c.IdCompra, c => c.NumeroFactura);
            ViewBag.ProductosNombres = productos.ToDictionary(p => p.idProducto, p => p.nombre);
        }

        public async Task<IActionResult> Index(
            int? compraId,
            int? productoId,
            decimal? precioMin,
            decimal? precioMax,
            int? cantidadMin,
            int? cantidadMax,
            int pageNumber = 1,
            int pageSize = 5)
        {
            await PopulateFilterDropdowns();

            IQueryable<DetalleCompraResponseDTO> query = _detalleCompraService.GetQueryable();

            if (compraId.HasValue && compraId.Value > 0)
            {
                query = query.Where(d => d.CompraId == compraId.Value);
            }

            if (productoId.HasValue && productoId.Value > 0)
            {
                query = query.Where(d => d.ProductoId == productoId.Value);
            }

            if (precioMin.HasValue)
            {
                query = query.Where(d => d.PrecioUnitarioCosto >= precioMin.Value);
            }

            if (precioMax.HasValue)
            {
                query = query.Where(d => d.PrecioUnitarioCosto <= precioMax.Value);
            }

            if (cantidadMin.HasValue)
            {
                query = query.Where(d => d.Cantidad >= cantidadMin.Value);
            }

            if (cantidadMax.HasValue)
            {
                query = query.Where(d => d.Cantidad <= cantidadMax.Value);
            }

            try
            {
                int totalRegistros = await query.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);

                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

                var listaPaginada = await query
                    .OrderByDescending(d => d.IdDetalleCompra)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentCompraId = compraId;
                ViewBag.CurrentProductoId = productoId;
                ViewBag.CurrentPrecioMin = precioMin;
                ViewBag.CurrentPrecioMax = precioMax;
                ViewBag.CurrentCantidadMin = cantidadMin;
                ViewBag.CurrentCantidadMax = cantidadMax;

                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;

                if (compraId.HasValue && compraId.Value > 0 ||
                    productoId.HasValue && productoId.Value > 0 ||
                    precioMin.HasValue || precioMax.HasValue ||
                    cantidadMin.HasValue || cantidadMax.HasValue)
                {
                    ViewData["IsFilterApplied"] = true;
                }

                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al cargar la lista de detalles de compra: " + ex.Message;
                await PopulateFilterDropdowns();

                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = 0;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;

                return View(new List<DetalleCompraResponseDTO>());
            }
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new DetalleCompraCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetalleCompraCreateDTO detalleCompraDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(detalleCompraDto);
            }

            try
            {
                var nuevoDetalle = await _detalleCompraService.AddAsync(detalleCompraDto);
                if (nuevoDetalle == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el detalle de compra.");
                    await PopulateDropdowns();
                    return View(detalleCompraDto);
                }

                TempData["Ok"] = "Detalle de compra creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException brex)
            {
                ModelState.AddModelError(string.Empty, brex.Message);
                await PopulateDropdowns();
                return View(detalleCompraDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el detalle de compra: " + ex.Message);
                await PopulateDropdowns();
                return View(detalleCompraDto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var detalleCompraDto = await _detalleCompraService.GetByIdAsync(id);
                if (detalleCompraDto == null)
                {
                    return NotFound();
                }

                var updateDto = new DetalleCompraUpdateDTO
                {
                    IdDetalleCompra = detalleCompraDto.IdDetalleCompra,
                    CompraId = detalleCompraDto.CompraId,
                    ProductoId = detalleCompraDto.ProductoId,
                    Cantidad = detalleCompraDto.Cantidad,
                    PrecioUnitarioCosto = detalleCompraDto.PrecioUnitarioCosto,
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
                TempData["ErrorMessage"] = "No se pudo cargar el detalle de compra para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DetalleCompraUpdateDTO detalleCompraDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(detalleCompraDto);
            }

            try
            {
                var success = await _detalleCompraService.UpdateAsync(id, detalleCompraDto);
                if (success)
                {
                    TempData["Ok"] = "Detalle de compra actualizado con éxito.";
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
                ModelState.AddModelError("", "Error al actualizar el detalle de compra. Verifique si el ID coincide o si la sesión es válida.");
            }

            await PopulateDropdowns();
            return View(detalleCompraDto);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var detalleCompra = await _detalleCompraService.GetByIdAsync(id);
                var compras = await _compraService.GetAllAsync();
                var productos = await _productoService.GetAllAsync();

                ViewBag.ComprasNombres = compras?.ToDictionary(c => c.IdCompra, c => c.NumeroFactura) ?? new Dictionary<int, string>();
                ViewBag.ProductosNombres = productos?.ToDictionary(p => p.idProducto, p => p.nombre) ?? new Dictionary<int, string>();

                if (detalleCompra == null)
                {
                    return NotFound();
                }

                return View(detalleCompra);
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var detalleCompra = await _detalleCompraService.GetByIdAsync(id);
                var compras = await _compraService.GetAllAsync();
                var productos = await _productoService.GetAllAsync();

                ViewBag.ComprasNombres = compras?.ToDictionary(c => c.IdCompra, c => c.NumeroFactura) ?? new Dictionary<int, string>();
                ViewBag.ProductosNombres = productos?.ToDictionary(p => p.idProducto, p => p.nombre) ?? new Dictionary<int, string>();

                return View(detalleCompra);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El detalle de compra solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _detalleCompraService.DeleteAsync(id);
                TempData["MensajeExito"] = "Detalle de compra eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El detalle de compra ya no existe o fue eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el detalle de compra: " + ex.Message);

                try
                {
                    var detalleCompra = await _detalleCompraService.GetByIdAsync(id);
                    var compras = await _compraService.GetAllAsync();
                    var productos = await _productoService.GetAllAsync();

                    ViewBag.ComprasNombres = compras?.ToDictionary(c => c.IdCompra, c => c.NumeroFactura) ?? new Dictionary<int, string>();
                    ViewBag.ProductosNombres = productos?.ToDictionary(p => p.idProducto, p => p.nombre) ?? new Dictionary<int, string>();

                    return View("Delete", detalleCompra);
                }
                catch (NotFoundException)
                {
                    TempData["MensajeError"] = "Error interno: El detalle de compra fue eliminado antes de mostrar el error.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // SOLO PDF - Acción para descargar PDF
        [HttpGet]
        public async Task<IActionResult> DescargarPdf(int id)
        {
            try
            {
                // Obtener datos
                var detalleCompra = await _detalleCompraService.GetByIdAsync(id);

                if (detalleCompra == null)
                {
                    return NotFound("No se encontró el detalle de compra.");
                }

                // Generar PDF
                var pdfGenerator = new DetalleCompraPDF();
                byte[] pdfBytes = pdfGenerator.GenerarArchivoFicha(detalleCompra);

                // Nombre del archivo
                string nombreArchivo = $"Compra_{detalleCompra.IdDetalleCompra}_{DateTime.Now:yyyyMMdd}.pdf";

                // Retornar archivo
                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                // Log del error (opcional)
                //Console.WriteLine($"Error al generar PDF: {ex.Message}");
                _logger.LogError(ex, "Error al generar PDF para ID: {Id}", id);

                // Redirigir o mostrar error
                TempData["ErrorMessage"] = "Error al generar el PDF.";
                return RedirectToAction("Index", "DetalleCompra");
            }
        }

        // OPCIÓN: Acción para ver PDF directamente en el navegador
        [HttpGet]
        public async Task<IActionResult> VerPdf(int id)
        {
            try
            {
                var detalleCompra = await _detalleCompraService.GetByIdAsync(id);

                if (detalleCompra == null)
                {
                    return NotFound();
                }

                var pdfGenerator = new DetalleCompraPDF();
                byte[] pdfBytes = pdfGenerator.GenerarArchivoFicha(detalleCompra);

                // Retornar sin nombre para visualizar en navegador
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al generar el PDF.";
                return RedirectToAction("Index", "DetalleCompra");
            }
        }




        // ACCIÓN PRINCIPAL PARA EXPORTAR EXCEL (SIMPLIFICADA)
        [HttpGet]
        public async Task<IActionResult> ExportarExcel(
            string numeroFactura = null,
            string nombreProducto = null, // Cambiado de productoId a nombreProducto
            int maxRegistros = 1000)
        {
            try
            {
                // Obtener consulta específica para Excel
                IQueryable<DetalleCompraExcelDTO> query = _detalleCompraService.GetQueryableForExcel();

                // APLICAR FILTROS DISPONIBLES
                if (!string.IsNullOrWhiteSpace(numeroFactura))
                {
                    query = query.Where(d => d.NumeroFactura.Contains(numeroFactura));
                }

                if (!string.IsNullOrWhiteSpace(nombreProducto))
                {
                    query = query.Where(d => d.nombre.Contains(nombreProducto));
                }

                // Limitar cantidad de registros
                query = query.Take(maxRegistros);

                // Obtener datos
                var datos = await query.ToListAsync();

                if (!datos.Any())
                {
                    TempData["ErrorMessage"] = "No hay compras para exportar con los filtros seleccionados.";
                    return RedirectToAction("Index");
                }

                // Generar Excel
                byte[] excelBytes = _excelExporter.ExportarDetalleCompra(datos);

                // Nombre del archivo
                string nombreArchivo = $"Compras_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    nombreArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al exportar Excel: {ex.Message}");
                TempData["ErrorMessage"] = $"Error al generar el Excel: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // ACCIÓN PARA FORMULARIO POST
        [HttpPost]
        public async Task<IActionResult> Exportar(
            string numeroFactura,
            string nombreProducto,
            int maxRegistros = 1000)
        {
            // Redirigir a la acción de exportación con parámetros
            return await ExportarExcel(
                numeroFactura: numeroFactura,
                nombreProducto: nombreProducto,
                maxRegistros: maxRegistros);
        }

        // ACCIÓN RÁPIDA: Exportar con filtros actuales del Index (SIMPLIFICADA)
        [HttpGet]
        public async Task<IActionResult> ExportarRapido()
        {
            try
            {
                // Obtener parámetros actuales del Index (solo los que existen en el DTO)
                string numeroFactura = Request.Query["numeroFactura"].ToString();
                string nombreProducto = Request.Query["nombreProducto"].ToString();

                // Redirigir a ExportarExcel con los parámetros disponibles
                return await ExportarExcel(
                    numeroFactura: !string.IsNullOrEmpty(numeroFactura) ? numeroFactura : null,
                    nombreProducto: !string.IsNullOrEmpty(nombreProducto) ? nombreProducto : null,
                    maxRegistros: 1000);
            }
            catch
            {
                // Si hay error, exportar todo
                return await ExportarExcel(maxRegistros: 1000);
            }
        }

        // VISTA PARA FORMULARIO DE EXPORTACIÓN (GET - SIMPLIFICADA)
        [HttpGet]
        public IActionResult Exportar()
        {
            // Opciones para límites
            ViewBag.MaxRegistrosOpciones = new List<int> { 100, 500, 1000, 5000, 10000 };

            return View();
        }


    }
}

