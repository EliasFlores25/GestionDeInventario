using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class UsuarioPropiedadesTests
    {
        [Fact]
        public void Usuario_PermiteAsignarDatosBasicos()
        {
            var usuario = new Usuario
            {
                nombre = "Stefanie",
                tipoRol = "Admin"
            };

            Assert.Equal("Stefanie", usuario.nombre);
            Assert.Equal("Admin", usuario.tipoRol);
        }

        [Fact]
        public void Usuario_IniciaConColecciones()
        {
            var usuario = new Usuario();

            Assert.NotNull(usuario.DetallesCompras);
            Assert.NotNull(usuario.DetallesDistribuciones);
        }
    }
}
