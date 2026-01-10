using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class EmpleadoTests
    {
        
        [Fact]
        public void Empleado_PuedeAsignarPropiedades()
        {
            // Arrange & Act
            var empleado = new Empleado
            {
                idEmpleado = 1,
                nombre = "Ana",
                apellido = "Pérez",
                edad = 25,
                genero = "Femenino",
                telefono = "78945612",
                direccion = "San Salvador",
                departamentoId = 2,
                estado = "Activo"
            };

            // Assert
            Assert.Equal(1, empleado.idEmpleado);
            Assert.Equal("Ana", empleado.nombre);
            Assert.Equal("Pérez", empleado.apellido);
            Assert.Equal(25, empleado.edad);
            Assert.Equal("Femenino", empleado.genero);
            Assert.Equal("78945612", empleado.telefono);
            Assert.Equal("San Salvador", empleado.direccion);
            Assert.Equal(2, empleado.departamentoId);
            Assert.Equal("Activo", empleado.estado);
        }

        
        [Fact]
        public void Empleado_DetallesDistribucion_Inicializado()
        {
            // Arrange
            var empleado = new Empleado();

            // Assert
            Assert.NotNull(empleado.DetallesDistribucion);
            Assert.Empty(empleado.DetallesDistribucion);
        }

        
        [Fact]
        public void Empleado_Departamento_PuedeAsignarse()
        {
            // Arrange
            var departamento = new Departamento
            {
                idDepartamento = 1,
                nombre = "Ventas"
            };

            var empleado = new Empleado
            {
                departamento = departamento
            };

            // Assert
            Assert.NotNull(empleado.departamento);
            Assert.Equal("Ventas", empleado.departamento.nombre);
        }
    }
}
