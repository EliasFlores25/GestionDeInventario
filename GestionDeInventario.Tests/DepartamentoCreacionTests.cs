using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class DepartamentoCreacionTests
    {
        [Fact]
        public void CrearDepartamento_SinErrores()
        {
            var departamento = new Departamento();

            Assert.NotNull(departamento);
        }

        [Fact]
        public void Departamento_IniciaConListaDeEmpleados()
        {
            var departamento = new Departamento();

            Assert.NotNull(departamento.Empleados);
        }
    }
}
