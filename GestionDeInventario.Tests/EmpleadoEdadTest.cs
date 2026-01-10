using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class EmpleadoEdadTest
    {
        [Fact]
        public void Empleado_Edad_EsValida()
        {
            var empleado = new Empleado { edad = 25 };

            Assert.True(empleado.edad >= 0);
        }

        [Fact]
        public void Empleado_PuedeTenerDepartamentoAsignado()
        {
            var empleado = new Empleado
            {
                departamento = new Departamento { nombre = "Ventas" }
            };

            Assert.NotNull(empleado.departamento);
        }
    }
}
