using GestionDeInventario.DTOs.DetalleCompraDTOs;
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
    public class DetalleCompraController : Controller
    {
        private readonly IDetalleCompraService _detalleCompraService;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;
        private readonly IProveedorService _proveedorService;
        private readonly ExcelExporter _excelExporter;
        public DetalleCompraController(IDetalleCompraService detalleCompraService, IUsuarioService usuarioService, IProductoService productoService, IProveedorService proveedorService, ExcelExporter excelExporter)
        {
            _detalleCompraService = detalleCompraService;
            _usuarioService = usuarioService;
            _productoService = productoService;
            _proveedorService = proveedorService;
            _excelExporter = excelExporter;
        }
        private async Task PopulateDropdownsUsuario()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            ViewBag.usuarioId = new SelectList(usuarios, "idUsuario", "nombre");
        }
        private async Task PopulateDropdownsProducto()
        {
            var productos = await _productoService.GetAllAsync();
            ViewBag.productoId = new SelectList(productos, "idProducto", "nombre");
        }
        private async Task PopulateDropdownsProveedor()
        {
            var proveedores = await _proveedorService.GetAllAsync();
            ViewBag.proveedorId = new SelectList(proveedores, "idProveedor", "nombreEmpresa");
        }
        private async Task PopulateUsuarioNamesViewBag()
        {
            var usuarios = await _usuarioService.GetAllAsync();

            var usuariosList = usuarios.Select(d => new SelectListItem
            {
                Value = d.idUsuario.ToString(),
                Text = d.nombre
            }).ToList();
            usuariosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Usuarios" });
            ViewBag.usuarioId = usuariosList;

            ViewBag.UsuariosNombres = usuarios.ToDictionary(d => d.idUsuario, d => d.nombre);
        }
        private async Task PopulateProductoNamesViewBag()
        {
            var productos = await _productoService.GetAllAsync();

            var productosList = productos.Select(d => new SelectListItem
            {
                Value = d.idProducto.ToString(),
                Text = d.nombre
            }).ToList();
            productosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Productos" });
            ViewBag.productoId = productosList;

            ViewBag.ProductosNombres = productos.ToDictionary(d => d.idProducto, d => d.nombre);
        }
        private async Task PopulateProveedorNamesViewBag()
        {
            var proveedores = await _proveedorService.GetAllAsync();

            var proveedoresList = proveedores.Select(d => new SelectListItem
            {
                Value = d.idProveedor.ToString(),
                Text = d.nombreEmpresa
            }).ToList();
            proveedoresList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Proveedores" });
            ViewBag.proveedorId = proveedoresList;

            ViewBag.ProveedoresNombres = proveedores.ToDictionary(d => d.idProveedor, d => d.nombreEmpresa);
        }

        public async Task<IActionResult> Index(string numeroFactura, DateTime? fechaCompra, int? productoId, int? proveedorId, int pageNumber = 1, int pageSize = 5)
        {
            await PopulateUsuarioNamesViewBag();
            await PopulateProductoNamesViewBag();
            await PopulateProveedorNamesViewBag();
            IQueryable<DetalleCompraResponseDTO> query = _detalleCompraService.GetQueryable();

            if (!string.IsNullOrWhiteSpace(numeroFactura))
            {
                string n_numeroFactura = numeroFactura.ToLower();
                query = query.Where(c => c.numeroFactura.ToLower().Contains(n_numeroFactura));
            }
            if (productoId is > 0)
                query = query.Where(c => c.productoId == productoId);
            if (proveedorId is > 0)
                query = query.Where(c => c.proveedorId == proveedorId);
            if (fechaCompra.HasValue)
                query = query.Where(c => c.fechaCompra.Date == fechaCompra.Value.Date);

            try
            {
                int totalRegistros = await query.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

                var listaPaginada = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.CurrentFactura = numeroFactura;
                ViewBag.CurrentFecha = fechaCompra;
                ViewBag.CurrentProducto = productoId;
                ViewBag.CurrentProveedor = proveedorId;

                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;

                if (!string.IsNullOrWhiteSpace(numeroFactura) || fechaCompra.HasValue || productoId > 0 || proveedorId > 0)
                {
                    ViewData["IsFilterApplied"] = true;
                }
                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la lista: " + ex.Message;

                ViewBag.PageNumber = 1;
                ViewBag.TotalPages = 1;
                return View(new List<DetalleCompraResponseDTO>());
            }
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsUsuario();
            await PopulateDropdownsProducto();
            await PopulateDropdownsProveedor();
            return View(new DetalleCompraCreateDTO());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetalleCompraCreateDTO detalleCompraDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsProveedor();
                return View(detalleCompraDto);
            }
            try
            {
                var nuevoDetalle = await _detalleCompraService.AddAsync(detalleCompraDto);
                if (nuevoDetalle == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el detalle de la compra.");
                    await PopulateDropdownsUsuario();
                    await PopulateDropdownsProducto();
                    await PopulateDropdownsProveedor();
                    return View(detalleCompraDto);
                }
                TempData["Ok"] = "Detalle de la compra creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException brex)
            {
                ModelState.AddModelError(string.Empty, brex.Message);
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsProveedor();
                return View(detalleCompraDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el detalle de la compra: " + ex.Message);
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsProveedor();
                return View(detalleCompraDto);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var detalleDto = await _detalleCompraService.GetByIdAsync(id);
                if (detalleDto == null)
                {
                    return NotFound();
                }
                var updateDto = new DetalleCompraUpdateDTO
                {
                    numeroFactura = detalleDto.numeroFactura,
                    usuarioId = detalleDto.usuarioId,
                    proveedorId = detalleDto.proveedorId,
                    productoId = detalleDto.productoId,
                    cantidad = detalleDto.cantidad,
                    precioUnitarioCosto = detalleDto.precioUnitarioCosto,
                    montoTotal = detalleDto.montoTotal,
                    fechaCompra = detalleDto.fechaCompra,
                };
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsProveedor();
                return View(updateDto);
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el detalle de la compra para edición.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DetalleCompraUpdateDTO detalleCompraUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsProveedor();
                return View(detalleCompraUpdateDTO);
            }
            try
            {
                var success = await  _detalleCompraService.UpdateAsync(id, detalleCompraUpdateDTO);
                if (success)
                {
                    TempData["Ok"] = "Detalle de la compra actualizado con éxito.";
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
                ModelState.AddModelError("", "Error al actualizar el detalle de la compra. Verifique si el ID coincide o si la sesión es válida.");
            }
            await PopulateDropdownsUsuario();
            await PopulateDropdownsProducto();
            await PopulateDropdownsProveedor();
            return View(detalleCompraUpdateDTO);
        }

        public async Task<IActionResult> Details(int id)
        {
            var detalleCompra = await _detalleCompraService.GetByIdAsync(id);
            var productos = await _productoService.GetAllAsync();
            var proveedores = await _proveedorService.GetAllAsync();
            var usuarios = await _usuarioService.GetAllAsync();

            ViewBag.ProductosNombres = productos?.ToDictionary(d => d.idProducto, d => d.nombre) ?? new Dictionary<int, string>();
            ViewBag.ProveedoresNombres = proveedores?.ToDictionary(d => d.idProveedor, d => d.nombreEmpresa) ?? new Dictionary<int, string>();
            ViewBag.UsuariosNombres = usuarios?.ToDictionary(d => d.idUsuario, d => d.nombre) ?? new Dictionary<int, string>();
            if (detalleCompra == null)
            {
                return NotFound();
            }
            return View(detalleCompra);
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var detalleCompra = await _detalleCompraService.GetByIdAsync(id);
                var productos = await _productoService.GetAllAsync();
                var proveedores = await _proveedorService.GetAllAsync();
                var usuarios = await _usuarioService.GetAllAsync();
                ViewBag.ProductosNombres = productos?.ToDictionary(d => d.idProducto, d => d.nombre) ?? new Dictionary<int, string>();
                ViewBag.ProveedoresNombres = proveedores?.ToDictionary(d => d.idProveedor, d => d.nombreEmpresa) ?? new Dictionary<int, string>();
                ViewBag.UsuariosNombres = usuarios?.ToDictionary(d => d.idUsuario, d => d.nombre) ?? new Dictionary<int, string>();
                return View(detalleCompra);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El detalle de la compra solicitado no existe.";
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
                TempData["MensajeExito"] = "Detalle de la compra eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El detalle de la compra ya no existe o fue eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el detalle de la compra: " + ex.Message);

                try
                {
                    var detalleCompra = await _detalleCompraService.GetByIdAsync(id);
                    var productos = await _productoService.GetAllAsync();
                    var proveedores = await _proveedorService.GetAllAsync();
                    var usuarios = await _usuarioService.GetAllAsync();
                    ViewBag.ProductosNombres = productos?.ToDictionary(d => d.idProducto, d => d.nombre) ?? new Dictionary<int, string>();
                    ViewBag.ProveedoresNombres = proveedores?.ToDictionary(d => d.idProveedor, d => d.nombreEmpresa) ?? new Dictionary<int, string>();
                    ViewBag.UsuariosNombres = usuarios?.ToDictionary(d => d.idUsuario, d => d.nombre) ?? new Dictionary<int, string>();
                    return View("Delete", detalleCompra);
                }
                catch (NotFoundException)
                {
                    TempData["MensajeError"] = "Error interno: El detalle de la compra fue eliminado antes de mostrar el error.";
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
                string nombreArchivo = $"Compra_{detalleCompra.numeroFactura}_{DateTime.Now:yyyyMMdd}.pdf";

                // Retornar archivo
                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                // Log del error (opcional)
                Console.WriteLine($"Error al generar PDF: {ex.Message}");

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




        // ACCIÓN PRINCIPAL PARA EXPORTAR EXCEL
        [HttpGet]
        public async Task<IActionResult> ExportarExcel(
            string numeroFactura = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            int? productoId = null,
            int? proveedorId = null,
            int maxRegistros = 1000)
        {
            try
            {
                // Obtener consulta específica para Excel
                IQueryable<DetalleCompraExcelDTO> query = _detalleCompraService.GetQueryableForExcel();

                // APLICAR FILTROS
                if (!string.IsNullOrWhiteSpace(numeroFactura))
                {
                    query = query.Where(d => d.NumeroFactura.Contains(numeroFactura));
                }

                if (fechaInicio.HasValue)
                {
                    query = query.Where(d => d.FechaCompra.Date >= fechaInicio.Value.Date);
                }

                if (fechaFin.HasValue)
                {
                    query = query.Where(d => d.FechaCompra.Date <= fechaFin.Value.Date);
                }

                if (productoId.HasValue && productoId > 0)
                {
                    query = query.Where(d => d.NombreProducto.Contains(productoId.ToString()));
                }

                if (proveedorId.HasValue && proveedorId > 0)
                {
                    query = query.Where(d => d.NombreProveedor.Contains(proveedorId.ToString()));
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

        // VISTA PARA FORMULARIO DE EXPORTACIÓN
        [HttpGet]
        public IActionResult Exportar()
        {
            // Opciones para límites
            ViewBag.MaxRegistrosOpciones = new List<int> { 100, 500, 1000, 5000, 10000 };

            // Puedes cargar listas para dropdowns si es necesario
            // ViewBag.Productos = await _productoService.GetAllAsync();
            // ViewBag.Proveedores = await _proveedorService.GetAllAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Exportar(
            string numeroFactura,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int? productoId,
            int? proveedorId,
            int maxRegistros = 1000)
        {
            // Redirigir a la acción de exportación con parámetros
            return RedirectToAction("ExportarExcel", new
            {
                numeroFactura,
                fechaInicio,
                fechaFin,
                productoId,
                proveedorId,
                maxRegistros
            });
        }

        // ACCIÓN RÁPIDA: Exportar con filtros actuales del Index
        [HttpGet]
        public async Task<IActionResult> ExportarRapido()
        {
            try
            {
                // Obtener parámetros actuales del Index
                string numeroFactura = Request.Query["numeroFactura"].ToString();
                DateTime? fechaCompra = Request.Query["fechaCompra"].Count > 0
                    ? DateTime.Parse(Request.Query["fechaCompra"])
                    : null;
                int? productoId = Request.Query["productoId"].Count > 0
                    ? int.Parse(Request.Query["productoId"])
                    : null;
                int? proveedorId = Request.Query["proveedorId"].Count > 0
                    ? int.Parse(Request.Query["proveedorId"])
                    : null;

                // Redirigir a ExportarExcel con los parámetros actuales
                return await ExportarExcel(
                    numeroFactura,
                    fechaCompra,
                    null, // fechaFin
                    productoId,
                    proveedorId,
                    1000);
            }
            catch
            {
                // Si hay error, exportar todo
                return await ExportarExcel(maxRegistros: 1000);
            }
        }

    }
}
