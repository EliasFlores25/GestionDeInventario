using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class ProductoExtraTests
    {
        [Fact]
        public void Producto_Precio_NoEsNegativo()
        {
            var producto = new Producto { precio = 10.50m };

            Assert.True(producto.precio >= 0);
        }

        [Fact]
        public void Producto_PermiteAgregarDetalleCompra()
        {
            var producto = new Producto();
            producto.detalleCompras.Add(new DetalleCompra());

            Assert.Single(producto.detalleCompras);
        }
    }
}
