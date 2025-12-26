using GestionDeInventario.DTOs.DetalleCompraDTOs;
using GestionDeInventario.DTOs.EmpleadoDTOs;
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

        public DetalleCompraController(IDetalleCompraService detalleCompraService, IUsuarioService usuarioService, IProductoService productoService, IProveedorService proveedorService)
        {
            _detalleCompraService = detalleCompraService;
            _usuarioService = usuarioService;
            _productoService = productoService;
            _proveedorService = proveedorService;
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
                return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
