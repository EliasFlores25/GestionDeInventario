using GestionDeInventario.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeInventario.Controllers
{
    public class DetalleCompraController : Controller
    {
      private readonly IDetalleCompraService _detalleCompraService;
        private readonly IUsuarioService _usuarioService;
        private readonly IProductoService _productoService;

        public DetalleCompraController(IDetalleCompraService detalleCompraService, IUsuarioService usuarioService, IProductoService productoService)
        {
            _detalleCompraService = detalleCompraService;
            _usuarioService = usuarioService;
            _productoService = productoService;
        }

    }
}
