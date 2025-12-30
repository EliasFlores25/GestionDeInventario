using GestionDeInventario.DTOs.DetalleDistribucionDTOs;
using GestionDeInventario.Services.Exceptions;
using GestionDeInventario.Services.Interfaces;
using GestionDeInventario.Utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionDeInventario.Controllers
{
    public class DetalleDistribucionController : Controller
    {
        private readonly IDetalleDistribucionService _detalleDistribucionService;
        private readonly IUsuarioService _usuarioService;
        private readonly IEmpleadoService _empleadoService;
        public readonly IProductoService _productoService;
        public DetalleDistribucionController(IDetalleDistribucionService dDetalleservice, IUsuarioService usuarioService, IEmpleadoService empleadoService, IProductoService productoService)
        {
            _detalleDistribucionService = dDetalleservice;
            _usuarioService = usuarioService;
            _empleadoService = empleadoService;
            _productoService = productoService;
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
        private async Task PopulateDropdownsEmpleado()
        {
            var empleados = await _empleadoService.GetAllAsync();
            ViewBag.empleadoId = new SelectList(empleados, "idEmpleado", "nombre");
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
        private async Task PopulateEmpleadoNamesViewBag()
        {
            var empleados = await _empleadoService.GetAllAsync();

            var empleadosList = empleados.Select(d => new SelectListItem
            {
                Value = d.idEmpleado.ToString(),
                Text = d.nombre
            }).ToList();
            empleadosList.Insert(0, new SelectListItem { Value = "", Text = "Todos los Empleados" });
            ViewBag.empleadoId = empleadosList;

            ViewBag.EmpleadosNombres = empleados.ToDictionary(d => d.idEmpleado, d => d.nombre);
        }

        public async Task<IActionResult> Index(string numeroDistribucion, DateTime? fechaSalida, int? empleadoId, int pageNumber = 1, int pageSize = 5)
        {
            await PopulateUsuarioNamesViewBag();
            await PopulateProductoNamesViewBag();
            await PopulateEmpleadoNamesViewBag();
            IQueryable<DetalleDistribucionResponseDTO> query = _detalleDistribucionService.GetQueryable();
            if (!string.IsNullOrWhiteSpace(numeroDistribucion))
            {
                string n_numeroDistribucion = numeroDistribucion.ToLower();
                query = query.Where(c => c.NumeroDistribucion.ToLower().Contains(n_numeroDistribucion));
            }
            if (fechaSalida.HasValue)
                query = query.Where(c => c.FechaSalida.Date == fechaSalida.Value.Date);
            if (empleadoId is > 0)
                query = query.Where(c => c.EmpleadoId == empleadoId);
            try
            {
                int totalRegistros = await query.CountAsync();
                int totalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
                pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages > 0 ? totalPages : 1));

                var listaPaginada = await query
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();

                ViewBag.CurrentFactura = numeroDistribucion;
                ViewBag.CurrentFecha = fechaSalida;
                ViewBag.CurrentProducto = empleadoId;

                ViewBag.PageNumber = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.HasPreviousPage = pageNumber > 1;
                ViewBag.HasNextPage = pageNumber < totalPages;

                if (!string.IsNullOrWhiteSpace(numeroDistribucion) || fechaSalida.HasValue || empleadoId > 0)
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
                return View(new List<DetalleDistribucionResponseDTO>());
            }
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsUsuario();
            await PopulateDropdownsProducto();
            await PopulateDropdownsEmpleado();
            return View(new DetalleDistribucionCreateDTO());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DetalleDistribucionCreateDTO detalleDistribucionDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsEmpleado();
                return View(detalleDistribucionDto);
            }
            try
            {
                var nuevoDetalle = await _detalleDistribucionService.AddAsync(detalleDistribucionDto);
                if (nuevoDetalle == null)
                {
                    ModelState.AddModelError("", "No se pudo crear el detalle de la distribución.");
                    await PopulateDropdownsUsuario();
                    await PopulateDropdownsProducto();
                    await PopulateDropdownsEmpleado();
                    return View(detalleDistribucionDto);
                }
                TempData["Ok"] = "Detalle de la distribución creado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException brex)
            {
                ModelState.AddModelError(string.Empty, brex.Message);
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsEmpleado();
                return View(detalleDistribucionDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el detalle de la distribución: " + ex.Message);
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsEmpleado();
                return View(detalleDistribucionDto);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var detalleDto = await _detalleDistribucionService.GetByIdAsync(id);
                if (detalleDto == null)
                {
                    return NotFound();
                }
                var updateDto = new DetalleDistribucionUpdateDTO
                {
                    NumeroDistribucion = detalleDto.NumeroDistribucion,
                    UsuarioId = detalleDto.UsuarioId,
                    EmpleadoId = detalleDto.EmpleadoId,
                    ProductoId = detalleDto.ProductoId,
                    Cantidad = detalleDto.Cantidad,
                    FechaSalida = detalleDto.FechaSalida,
                    Motivo = detalleDto.Motivo,
                    MontoTotal = detalleDto.MontoTotal,
                    PrecioCostoUnitario = detalleDto.PrecioCostoUnitario
                };
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsEmpleado();
                return View(updateDto);
            }
            catch (NotFoundException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "No se pudo cargar el detalle de la distribución para edición.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DetalleDistribucionUpdateDTO detalleDistribucionUpdateDTO)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsUsuario();
                await PopulateDropdownsProducto();
                await PopulateDropdownsEmpleado();
                return View(detalleDistribucionUpdateDTO);
            }
            try
            {
                var success = await _detalleDistribucionService.UpdateAsync(id, detalleDistribucionUpdateDTO);
                if (success)
                {
                    TempData["Ok"] = "Detalle de la distribución actualizado con éxito.";
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
                ModelState.AddModelError("", "Error al actualizar el detalle de la distribución. Verifique si el ID coincide o si la sesión es válida.");
            }
            await PopulateDropdownsUsuario();
            await PopulateDropdownsProducto();
            await PopulateDropdownsEmpleado();
            return View(detalleDistribucionUpdateDTO);
        }
        public async Task<IActionResult> Details(int id)
        {
            var detalleDistribucion = await _detalleDistribucionService.GetByIdAsync(id);
            var empleados = await _empleadoService.GetAllAsync();
            var productos = await _productoService.GetAllAsync();
            var usuarios = await _usuarioService.GetAllAsync();

            ViewBag.EmpleadosNombres = empleados?.ToDictionary(d => d.idEmpleado, d => d.nombre) ?? new Dictionary<int, string>();
            ViewBag.ProductosNombres = productos?.ToDictionary(d => d.idProducto, d => d.nombre) ?? new Dictionary<int, string>();
            ViewBag.UsuariosNombres = usuarios?.ToDictionary(d => d.idUsuario, d => d.nombre) ?? new Dictionary<int, string>();
            if (detalleDistribucion == null)
            {
                return NotFound();
            }
            return View(detalleDistribucion);
        }
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var detalleDistribucion = await _detalleDistribucionService.GetByIdAsync(id);
                var empleados = await _empleadoService.GetAllAsync();
                var productos = await _productoService.GetAllAsync();
                var usuarios = await _usuarioService.GetAllAsync();
                ViewBag.EmpleadosNombres = empleados?.ToDictionary(d => d.idEmpleado, d => d.nombre) ?? new Dictionary<int, string>();
                ViewBag.ProductosNombres = productos?.ToDictionary(d => d.idProducto, d => d.nombre) ?? new Dictionary<int, string>();
                ViewBag.UsuariosNombres = usuarios?.ToDictionary(d => d.idUsuario, d => d.nombre) ?? new Dictionary<int, string>();
                return View(detalleDistribucion);
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El detalle de la distribución solicitado no existe.";
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
                TempData["MensajeExito"] = "Detalle de la distribución eliminado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                TempData["MensajeError"] = "Error: El detalle de la distribución ya no existe o fue eliminado.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el detalle de la distribución: " + ex.Message);

                try
                {
                    var detalleDistribucion = await _detalleDistribucionService.GetByIdAsync(id);
                    var empleados = await _empleadoService.GetAllAsync();
                    var productos = await _productoService.GetAllAsync();
                    var usuarios = await _usuarioService.GetAllAsync();
                    ViewBag.EmpleadosNombres = empleados?.ToDictionary(d => d.idEmpleado, d => d.nombre) ?? new Dictionary<int, string>();
                    ViewBag.ProductosNombres = productos?.ToDictionary(d => d.idProducto, d => d.nombre) ?? new Dictionary<int, string>();
                    ViewBag.UsuariosNombres = usuarios?.ToDictionary(d => d.idUsuario, d => d.nombre) ?? new Dictionary<int, string>();
                    return View(detalleDistribucion);
                }
                catch (NotFoundException)
                {
                    TempData["MensajeError"] = "Error interno: El detalle de la distribución fue eliminado antes de mostrar el error.";
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
                var detalleDistribucion = await _detalleDistribucionService.GetByIdAsync(id);

                if (detalleDistribucion == null)
                {
                    return NotFound("No se encontró el detalle de distribución.");
                }

                // Generar PDF
                var pdfGenerator = new DetalleDistribucionPDF();
                byte[] pdfBytes = pdfGenerator.GenerarArchivoFicha(detalleDistribucion);

                // Nombre del archivo
                string nombreArchivo = $"Distribucion_{detalleDistribucion.NumeroDistribucion}_{DateTime.Now:yyyyMMdd}.pdf";

                // Retornar archivo
                return File(pdfBytes, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                // Log del error
                Console.WriteLine($"Error al generar PDF: {ex.Message}");

                // Redirigir o mostrar error
                TempData["ErrorMessage"] = "Error al generar el PDF.";
                return RedirectToAction("Index", "DetalleDistribucion");
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
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al generar el PDF.";
                return RedirectToAction("Index", "DetalleDistribucion");
            }
        }
    }
}