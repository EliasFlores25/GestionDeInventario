using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class DepartamentoTests
    {
        // 1️⃣ Verifica que las propiedades se asignan correctamente
        [Fact]
        public void Departamento_PuedeAsignarPropiedades()
        {
            // Arrange & Act
            var departamento = new Departamento
            {
                idDepartamento = 1,
                nombre = "Recursos Humanos",
                descripcion = "Departamento de RRHH"
            };

            // Assert
            Assert.Equal(1, departamento.idDepartamento);
            Assert.Equal("Recursos Humanos", departamento.nombre);
            Assert.Equal("Departamento de RRHH", departamento.descripcion);
        }

        // 2️⃣ Verifica que la colección Empleados se inicializa
        [Fact]
        public void Departamento_Empleados_InicializadoCorrectamente()
        {
            // Arrange
            var departamento = new Departamento();

            // Assert
            Assert.NotNull(departamento.Empleados);
            Assert.Empty(departamento.Empleados);
        }
    }
}
