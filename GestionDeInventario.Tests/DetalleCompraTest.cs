using GestionDeInventario.Models;
using System;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class DetalleCompraTests
    {
       
        [Fact]
        public void DetalleCompra_PuedeAsignarPropiedades()
        {
            // Arrange & Act
            var detalle = new DetalleCompra
            {
                idDetalleCompra = 1,
                numeroFactura = "FAC-001",
                cantidad = 20,
                precioUnitarioCosto = 3.75m,
                fechaCompra = DateTime.Today,
                usuarioId = 2,
                productoId = 3,
                proveedorId = 4
            };

            // Assert
            Assert.Equal(1, detalle.idDetalleCompra);
            Assert.Equal("FAC-001", detalle.numeroFactura);
            Assert.Equal(20, detalle.cantidad);
            Assert.Equal(3.75m, detalle.precioUnitarioCosto);
            Assert.Equal(2, detalle.usuarioId);
            Assert.Equal(3, detalle.productoId);
            Assert.Equal(4, detalle.proveedorId);
        }

        [Fact]
        public void DetalleCompra_PuedeAsignarRelaciones()
        {
            // Arrange
            var usuario = new Usuario { idUsuario = 1, nombre = "Admin" };
            var producto = new Producto { idProducto = 2, nombre = "Arroz" };
            var proveedor = new Proveedor { idProveedor = 3, nombreEmpresa = "Proveedor S.A." };

            // Act
            var detalle = new DetalleCompra
            {
                usuario = usuario,
                producto = producto,
                proveedor = proveedor
            };

            // Assert
            Assert.NotNull(detalle.usuario);
            Assert.NotNull(detalle.producto);
            Assert.NotNull(detalle.proveedor);
            Assert.Equal("Admin", detalle.usuario.nombre);
            Assert.Equal("Arroz", detalle.producto.nombre);
            Assert.Equal("Proveedor S.A.", detalle.proveedor.nombreEmpresa);
        }

        
        [Fact]
        public void DetalleCompra_MontoTotal_Existe()
        {
            // Arrange
            var detalle = new DetalleCompra();

            // Act
            var monto = detalle.montoTotal;

            // Assert
            Assert.True(monto >= 0);
        }
    }
}
