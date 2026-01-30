using GestionDeInventario.DTOs.DetalleDistribucionDTOs;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Implementations;
using GestionDeInventario.Services.Interfaces;
using GestionDeInventario.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DetalleDistribucionController : Controller
    {
        private readonly IDetalleDistribucionService _detalleDistribucionService;
        private readonly IDistribucionService _distribucionService;
        private readonly IProductoService _productoService;
        private readonly ILogger<DetalleDistribucionController> _logger;
        private readonly ExcelExporter _excelExporter;

        public DetalleDistribucionController(
            IDetalleDistribucionService detalleDistribucionService,
            IDistribucionService distribucionService,
            IProductoService productoService,
            ExcelExporter excelExporter,
            ILogger<DetalleDistribucionController> logger)
        {
            _detalleDistribucionService = detalleDistribucionService;
            _distribucionService = distribucionService;
            _productoService = productoService;
            _excelExporter = excelExporter;
            _logger = logger;
        }

        private async Task PopulateDropdowns()
        {
            var distribuciones = await _distribucionService.GetAllAsync();
            var productos = await _productoService.GetAllAsync();

            ViewBag.DistribucionId = new SelectList(distribuciones, "IdDistribucion", "NumeroDistribucion");
            ViewBag.ProductoId = new SelectList(productos, "idProducto", "nombre");
        }

        private async Task PopulateFilterDropdowns()
        {
            var distribuciones = await _distribucionService.GetAllAsync();
            var productos = await _productoService.GetAllAsync();

            // Para filtros
            var distribucionesList = distribuciones.Select(d => new SelectListItem
            {
                Value = d.IdDistribucion.ToString(),
                Text = d.NumeroDistribucion
            }).ToList();
            distribucionesList.Insert(0, new SelectListItem { Value = "", Text = "Todas las Distribuciones" });
            ViewBag.DistribucionId = distribucionesList;

            var productosList = productos.Select(p => new SelectListItem
            {
                Value = p.idProducto.ToString(),
                Text = p.nombre
            }).ToList();
            productosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Productos" });
            ViewBag.ProductoId = productosList;

            // Para mostrar nombres en vistas
            ViewBag.DistribucionesNombres = distribuciones.ToDictionary(d => d.IdDistribucion, d => d.NumeroDistribucion);
            ViewBag.ProductosNombres = productos.ToDictionary(p => p.idProducto, p => p.nombre);
        }

        public async Task<IActionResult> Index(
            int? distribucionId,
            int? productoId,
            int? cantidadMin,
            int? cantidadMax,
            int pageNumber = 1,
            int pageSize = 5)
        {
            await PopulateFilterDropdowns();

            IQueryable<DetalleDistribucionResponseDTO> query = _detalleDistribucionService.GetQueryable();

            if (distribucionId.HasValue && distribucionId.Value > 0)
            {
                query = query.Where(d => d.DistribucionId == distribucionId.Value);
            }

            if (productoId.HasValue && productoId.Value > 0)
            {
                query = query.Where(d => d.ProductoId == productoId.Value);
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
                    .OrderByDescending(d => d.IdDetalleDistribucion)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentDistribucionId = distribucionId;
                ViewBag.CurrentProductoId = productoId;
                ViewBag.CurrentCantidadMin = cantidadMin;
                ViewBag.CurrentCantidadMax = cantidadMax;

                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;

                if (distribucionId.HasValue && distribucionId.Value > 0 ||
                    productoId.HasValue && productoId.Value > 0 ||
                    cantidadMin.HasValue || cantidadMax.HasValue)
                {
                    ViewData["IsFilterApplied"] = true;
                }

                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al cargar la lista de detalles de distribución: " + ex.Message;
                await PopulateFilterDropdowns();

                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = 0;
                ViewBag.HasPreviousPage = false;
                ViewBag.HasNextPage = false;

                return View(new List<DetalleDistribucionResponseDTO>());
            }
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new DetalleDistribucionCreateDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetalleDistribucionCreateDTO detalleDistribucionDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(detalleDistribucionDto);
            }

            try
            {
                var nuevoDetalle = await _detalleDistribucionService.AddAsync(detalleDistribucionDto);
                if (nuevoDetalle == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el detalle de distribución.");
                    await PopulateDropdowns();
                    return View(detalleDistribucionDto);
                }

                TempData["Ok"] = "Detalle de distribución creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException brex)
            {
                ModelState.AddModelError(string.Empty, brex.Message);
                await PopulateDropdowns();
                return View(detalleDistribucionDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el detalle de distribución: " + ex.Message);
                await PopulateDropdowns();
                return View(detalleDistribucionDto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var detalleDistribucionDto = await _detalleDistribucionService.GetByIdAsync(id);
                if (detalleDistribucionDto == null)
                {
                    return NotFound();
                }

                var updateDto = new DetalleDistribucionUpdateDTO
                {
                    IdDetalleDistribucion = detalleDistribucionDto.IdDetalleDistribucion,
                    DistribucionId = detalleDistribucionDto.DistribucionId,
                    ProductoId = detalleDistribucionDto.ProductoId,
                    Cantidad = detalleDistribucionDto.Cantidad,
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
                TempData["ErrorMessage"] = "No se pudo cargar el detalle de distribución para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DetalleDistribucionUpdateDTO detalleDistribucionDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(detalleDistribucionDto);
            }

            try
            {
                var success = await _detalleDistribucionService.UpdateAsync(id, detalleDistribucionDto);
                if (success)
                {
                    TempData["Ok"] = "Detalle de distribución actualizado con éxito.";
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
                ModelState.AddModelError("", "Error al actualizar el detalle de distribución. Verifique si el ID coincide o si la sesión es válida.");
            }

            await PopulateDropdowns();
            return View(detalleDistribucionDto);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var detalleDistribucion = await _detalleDistribucionService.GetByIdAsync(id);
                var distribuciones = await _distribucionService.GetAllAsync();
                var productos = await _productoService.GetAllAsync();

                ViewBag.DistribucionesNombres = distribuciones?.ToDictionary(d => d.IdDistribucion, d => d.NumeroDistribucion) ?? new Dictionary<int, string>();
                ViewBag.ProductosNombres = productos?.ToDictionary(p => p.idProducto, p => p.nombre) ?? new Dictionary<int, string>();

                if (detalleDistribucion == null)
                {
                    return NotFound();
                }

                return View(detalleDistribucion);
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
                var detalleDistribucion = await _detalleDistribucionService.GetByIdAsync(id);
                var distribuciones = await _distribucionService.GetAllAsync();
                var productos = await _productoService.GetAllAsync();

                ViewBag.DistribucionesNombres = distribuciones?.ToDictionary(d => d.IdDistribucion, d => d.NumeroDistribucion) ?? new Dictionary<int, string>();
                ViewBag.ProductosNombres = productos?.ToDictionary(p => p.idProducto, p => p.nombre) ?? new Dictionary<int, string>();

                return View(detalleDistribucion);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El detalle de distribución solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _detalleDistribucionService.DeleteAsync(id);
                TempData["MensajeExito"] = "Detalle de distribución eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El detalle de distribución ya no existe o fue eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el detalle de distribución: " + ex.Message);

                try
                {
                    var detalleDistribucion = await _detalleDistribucionService.GetByIdAsync(id);
                    var distribuciones = await _distribucionService.GetAllAsync();
                    var productos = await _productoService.GetAllAsync();

                    ViewBag.DistribucionesNombres = distribuciones?.ToDictionary(d => d.IdDistribucion, d => d.NumeroDistribucion) ?? new Dictionary<int, string>();
                    ViewBag.ProductosNombres = productos?.ToDictionary(p => p.idProducto, p => p.nombre) ?? new Dictionary<int, string>();

                    return View("Delete", detalleDistribucion);
                }
                catch (NotFoundException)
                {
                    TempData["MensajeError"] = "Error interno: El detalle de distribución fue eliminado antes de mostrar el error.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // Métodos para Excel

        // ACCIÓN PRINCIPAL PARA EXPORTAR EXCEL (SIN FECHAS)
        [HttpGet]
        public async Task<IActionResult> ExportarExcel(
            string numeroDistribucion = null,
            string nombreProducto = null,
            int maxRegistros = 1000)
        {
            try
            {
                // Obtener consulta específica para Excel
                IQueryable<DetalleDistribucionExcelDTO> query = _detalleDistribucionService.GetQueryableForExcel();

                // APLICAR FILTROS DISPONIBLES
                if (!string.IsNullOrWhiteSpace(numeroDistribucion))
                {
                    query = query.Where(d => d.NumeroDistribucion.Contains(numeroDistribucion));
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
                    TempData["ErrorMessage"] = "No hay distribuciones para exportar con los filtros seleccionados.";
                    return RedirectToAction("Index");
                }

                // Generar Excel
                byte[] excelBytes = _excelExporter.ExportarDetalleDistribucion(datos);

                // Nombre del archivo
                string nombreArchivo = $"Distribuciones_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    nombreArchivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar Excel de distribuciones");
                TempData["ErrorMessage"] = $"Error al generar el Excel: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // ACCIÓN PARA FORMULARIO POST (SIN FECHAS)
        [HttpPost]
        public async Task<IActionResult> Exportar(
            string numeroDistribucion,
            string nombreProducto,
            int maxRegistros = 1000)
        {
            // Redirigir a la acción de exportación con parámetros
            return await ExportarExcel(
                numeroDistribucion: numeroDistribucion,
                nombreProducto: nombreProducto,
                maxRegistros: maxRegistros);
        }

        // ACCIÓN RÁPIDA: Exportar con filtros actuales del Index (SIN FECHAS)
        [HttpGet]
        public async Task<IActionResult> ExportarRapido()
        {
            try
            {
                // Obtener parámetros actuales del Index
                string numeroDistribucion = Request.Query["numeroDistribucion"].ToString();
                string nombreProducto = Request.Query["nombreProducto"].ToString();

                // Redirigir a ExportarExcel con los parámetros disponibles
                return await ExportarExcel(
                    numeroDistribucion: !string.IsNullOrEmpty(numeroDistribucion) ? numeroDistribucion : null,
                    nombreProducto: !string.IsNullOrEmpty(nombreProducto) ? nombreProducto : null,
                    maxRegistros: 1000);
            }
            catch
            {
                // Si hay error, exportar todo
                return await ExportarExcel(maxRegistros: 1000);
            }
        }

        // VISTA PARA FORMULARIO DE EXPORTACIÓN (GET) - SIN REFERENCIAS A FECHAS
        [HttpGet]
        public IActionResult Exportar()
        {
            // Opciones para límites
            ViewBag.MaxRegistrosOpciones = new List<int> { 100, 500, 1000, 5000, 10000 };

            // QUITADO: No hay referencias a fechas
            // ViewBag.FechaInicioDefault = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            // ViewBag.FechaFinDefault = DateTime.Now.ToString("yyyy-MM-dd");

            return View();
        }



        // ==================== MÉTODOS PDF ====================

        // SOLO PDF - Acción para descargar PDF
        [HttpGet]
        public async Task<IActionResult> DescargarPdf(int id)
        {
            try
            {
                // Obtener datos
                var detalleDistribucion = await _detalleDistribucionService.GetByIdAsync(id);

                if (detalleDistribucion == null)
                {
                    return NotFound("No se encontró el detalle de distribución.");
                }

                // Generar PDF - NOTA: Asegúrate de que la clase DetalleDistribucionPDF existe
                var pdfGenerator = new DetalleDistribucionPDF();
                byte[] pdfBytes = pdfGenerator.GenerarArchivoFicha(detalleDistribucion);

                // Nombre del archivo (CORREGIDO: Cambié "Compra_" por "DetalleDistribucion_")
                string nombreArchivo = $"DetalleDistribucion_{detalleDistribucion.IdDetalleDistribucion}_{DateTime.Now:yyyyMMdd}.pdf";

                // Retornar archivo
                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF para ID: {Id}", id);
                TempData["ErrorMessage"] = $"Error al generar el PDF: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // OPCIÓN: Acción para ver PDF directamente en el navegador
        [HttpGet]
        public async Task<IActionResult> VerPdf(int id)
        {
            try
            {
                var detalleDistribucion = await _detalleDistribucionService.GetByIdAsync(id);

                if (detalleDistribucion == null)
                {
                    return NotFound();
                }

                var pdfGenerator = new DetalleDistribucionPDF();
                byte[] pdfBytes = pdfGenerator.GenerarArchivoFicha(detalleDistribucion);

                // Retornar sin nombre para visualizar en navegador
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF para ID: {Id}", id);
                TempData["ErrorMessage"] = $"Error al generar el PDF: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}