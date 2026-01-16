using GestionDeInventario.Models;
using Xunit;
using System;

namespace GestionDeInventario.Tests.Models
{
    public class DetalleCompraValoresTests
    {
        [Fact]
        public void DetalleCompra_AdmiteCantidadYPrecio()
        {
            var detalle = new DetalleCompra
            {
                cantidad = 10,
                precioUnitarioCosto = 2.5m
            };

            Assert.Equal(10, detalle.cantidad);
            Assert.Equal(2.5m, detalle.precioUnitarioCosto);
        }

        [Fact]
        public void DetalleCompra_AdmiteFechaCompra()
        {
            var fecha = DateTime.Today;

            var detalle = new DetalleCompra
            {
                fechaCompra = fecha
            };

            Assert.Equal(fecha, detalle.fechaCompra);
        }
    }
}
