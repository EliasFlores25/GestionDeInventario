using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class DetalleCompraAministrarTests
    {
        [Fact]
        public void DetalleCompra_Cantidad_EsMayorACero()
        {
            var detalle = new DetalleCompra { cantidad = 5 };

            Assert.True(detalle.cantidad > 0);
        }

        [Fact]
        public void DetalleCompra_TieneProveedorAsignado()
        {
            var detalle = new DetalleCompra
            {
                proveedor = new Proveedor { nombreEmpresa = "Proveedor Y" }
            };

            Assert.NotNull(detalle.proveedor);
        }
    }
}
