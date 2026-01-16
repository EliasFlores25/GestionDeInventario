using GestionDeInventario.Models;
using Xunit;
using System;

namespace GestionDeInventario.Tests.Models
{
    public class DetalleDistribucionDatosTests
    {
        [Fact]
        public void DetalleDistribucion_RegistraCantidad()
        {
            var detalle = new DetalleDistribucion
            {
                Cantidad = 4
            };

            Assert.Equal(4, detalle.Cantidad);
        }

        [Fact]
        public void DetalleDistribucion_RegistraFechaSalida()
        {
            var fecha = DateTime.Now;

            var detalle = new DetalleDistribucion
            {
                FechaSalida = fecha
            };

            Assert.Equal(fecha, detalle.FechaSalida);
        }
    }
}
