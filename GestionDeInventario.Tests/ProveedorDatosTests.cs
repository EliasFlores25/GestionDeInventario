using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class ProveedorDatosTests
    {
        [Fact]
        public void Proveedor_PermiteAsignarEmpresa()
        {
            var proveedor = new Proveedor
            {
                nombreEmpresa = "Proveedor SA"
            };

            Assert.Equal("Proveedor SA", proveedor.nombreEmpresa);
        }

        [Fact]
        public void Proveedor_IniciaListaDeCompras()
        {
            var proveedor = new Proveedor();

            Assert.NotNull(proveedor.detalleCompras);
        }
    }
}
