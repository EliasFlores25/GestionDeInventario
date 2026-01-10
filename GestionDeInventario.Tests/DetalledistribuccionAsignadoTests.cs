using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class DetalleDistribucionAsignadoTests
    {
        [Fact]
        public void DetalleDistribucion_Cantidad_EsMayorACero()
        {
            var detalle = new DetalleDistribucion { Cantidad = 3 };

            Assert.True(detalle.Cantidad > 0);
        }

        [Fact]
        public void DetalleDistribucion_TieneProductoAsignado()
        {
            var detalle = new DetalleDistribucion
            {
                Producto = new Producto { nombre = "Mouse" }
            };

            Assert.NotNull(detalle.Producto);
        }
    }
}
