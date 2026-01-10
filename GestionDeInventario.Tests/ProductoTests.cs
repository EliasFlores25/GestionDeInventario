using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class ProductoTests
    {
        [Fact]
        public void Producto_PuedeAsignarPropiedadesCorrectamente()
        {
            // Arrange & Act
            var producto = new Producto
            {
                idProducto = 1,
                nombre = "Arroz",
                descripcion = "Arroz blanco",
                cantidadStock = 100,
                unidadMedida = "Kg",
                precio = 1.25m,
                estado = "Activo"
            };

            // Assert
            Assert.Equal(1, producto.idProducto);
            Assert.Equal("Arroz", producto.nombre);
            Assert.Equal("Arroz blanco", producto.descripcion);
            Assert.Equal(100, producto.cantidadStock);
            Assert.Equal("Kg", producto.unidadMedida);
            Assert.Equal(1.25m, producto.precio);
            Assert.Equal("Activo", producto.estado);
        }
        [Fact]
        public void Producto_Colecciones_DebenInicializarse()
        {
            // Act
            var producto = new Producto();

            // Assert
            Assert.NotNull(producto.detalleCompras);
            Assert.NotNull(producto.detalleDistribuciones);
            Assert.Empty(producto.detalleCompras);
            Assert.Empty(producto.detalleDistribuciones);
        }
    }
}

