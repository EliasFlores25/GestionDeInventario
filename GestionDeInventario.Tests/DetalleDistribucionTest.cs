using GestionDeInventario.Models;
using System;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class DetalleDistribucionTests
    {
        
        [Fact]
        public void DetalleDistribucion_PuedeAsignarPropiedades()
        {
            // Arrange & Act
            var detalle = new DetalleDistribucion
            {
                IdDetalleDistribucion = 1,
                NumeroDistribucion = "DIST-001",
                UsuarioId = 2,
                EmpleadoId = 3,
                ProductoId = 4,
                Cantidad = 10,
                FechaSalida = DateTime.Today,
                Motivo = "Entrega a área operativa",
                PrecioCostoUnitario = 5.50m
            };

            // Assert
            Assert.Equal(1, detalle.IdDetalleDistribucion);
            Assert.Equal("DIST-001", detalle.NumeroDistribucion);
            Assert.Equal(2, detalle.UsuarioId);
            Assert.Equal(3, detalle.EmpleadoId);
            Assert.Equal(4, detalle.ProductoId);
            Assert.Equal(10, detalle.Cantidad);
            Assert.Equal("Entrega a área operativa", detalle.Motivo);
            Assert.Equal(5.50m, detalle.PrecioCostoUnitario);
        }

        
        [Fact]
        public void DetalleDistribucion_PuedeAsignarRelaciones()
        {
            // Arrange
            var usuario = new Usuario { idUsuario = 1, nombre = "Admin" };
            var empleado = new Empleado { idEmpleado = 2, nombre = "Carlos" };
            var producto = new Producto { idProducto = 3, nombre = "Laptop" };

            // Act
            var detalle = new DetalleDistribucion
            {
                Usuario = usuario,
                Empleado = empleado,
                Producto = producto
            };

            // Assert
            Assert.NotNull(detalle.Usuario);
            Assert.NotNull(detalle.Empleado);
            Assert.NotNull(detalle.Producto);
            Assert.Equal("Admin", detalle.Usuario.nombre);
            Assert.Equal("Carlos", detalle.Empleado.nombre);
            Assert.Equal("Laptop", detalle.Producto.nombre);
        }

        [Fact]
        public void DetalleDistribucion_MontoTotal_Existe()
        {
            // Arrange
            var detalle = new DetalleDistribucion();

            // Act
            var monto = detalle.MontoTotal;

            // Assert
            Assert.True(monto >= 0);
        }
    }
}
