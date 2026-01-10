using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class ProveedorTests
    {
       
        [Fact]
        public void Proveedor_PuedeAsignarPropiedades()
        {
            // Arrange & Act
            var proveedor = new Proveedor
            {
                idProveedor = 1,
                nombreEmpresa = "Distribuidora El Salvador",
                direccion = "San Salvador",
                telefono = "2233-4455",
                email = "contacto@proveedor.com",
                estado = "Activo"
            };

            // Assert
            Assert.Equal(1, proveedor.idProveedor);
            Assert.Equal("Distribuidora El Salvador", proveedor.nombreEmpresa);
            Assert.Equal("San Salvador", proveedor.direccion);
            Assert.Equal("2233-4455", proveedor.telefono);
            Assert.Equal("contacto@proveedor.com", proveedor.email);
            Assert.Equal("Activo", proveedor.estado);
        }

        
        [Fact]
        public void Proveedor_DetalleCompras_Inicializado()
        {
            // Arrange
            var proveedor = new Proveedor();

            // Assert
            Assert.NotNull(proveedor.detalleCompras);
            Assert.Empty(proveedor.detalleCompras);
        }
    }
}
