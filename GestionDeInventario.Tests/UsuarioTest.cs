using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class UsuarioTests
    {
        // 1️⃣ Verificar asignación de propiedades
        [Fact]
        public void Usuario_PuedeAsignarPropiedades()
        {
            // Arrange & Act
            var usuario = new Usuario
            {
                idUsuario = 1,
                nombre = "Juan Perez",
                tipoRol = "Administrador",
                email = "juan@correo.com",
                contraseña = "123456"
            };

            // Assert
            Assert.Equal(1, usuario.idUsuario);
            Assert.Equal("Juan Perez", usuario.nombre);
            Assert.Equal("Administrador", usuario.tipoRol);
            Assert.Equal("juan@correo.com", usuario.email);
            Assert.Equal("123456", usuario.contraseña);
        }

        // 2️⃣ Verificar inicialización de DetallesCompras
        [Fact]
        public void Usuario_DetallesCompras_Inicializado()
        {
            // Arrange
            var usuario = new Usuario();

            // Assert
            Assert.NotNull(usuario.DetallesCompras);
            Assert.Empty(usuario.DetallesCompras);
        }

        // 3️⃣ Verificar inicialización de DetallesDistribuciones
        [Fact]
        public void Usuario_DetallesDistribuciones_Inicializado()
        {
            // Arrange
            var usuario = new Usuario();

            // Assert
            Assert.NotNull(usuario.DetallesDistribuciones);
            Assert.Empty(usuario.DetallesDistribuciones);
        }
    }
}
