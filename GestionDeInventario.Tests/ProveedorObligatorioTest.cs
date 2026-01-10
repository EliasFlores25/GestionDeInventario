using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class ProveedorExtraTests
    {
        [Fact]
        public void Proveedor_NombreEmpresa_EsObligatorio()
        {
            var proveedor = new Proveedor { nombreEmpresa = "Proveedor X" };

            Assert.False(string.IsNullOrWhiteSpace(proveedor.nombreEmpresa));
        }

        [Fact]
        public void Proveedor_PermiteAgregarDetalleCompra()
        {
            var proveedor = new Proveedor();
            proveedor.detalleCompras.Add(new DetalleCompra());

            Assert.Single(proveedor.detalleCompras);
        }
    }
}
