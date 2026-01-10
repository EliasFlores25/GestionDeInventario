using GestionDeInventario.Models;
using Xunit;

namespace GestionDeInventario.Tests.Models
{
    public class DepartamentoEmpleadoTests
    {
        [Fact]
        public void Departamento_PermiteAgregarEmpleados()
        {
            var departamento = new Departamento();
            departamento.Empleados.Add(new Empleado { nombre = "Ana" });

            Assert.Single(departamento.Empleados);
        }

        [Fact]
        public void Departamento_Nombre_NoDebeSerVacio()
        {
            var departamento = new Departamento { nombre = "Bodega" };

            Assert.False(string.IsNullOrWhiteSpace(departamento.nombre));
        }
    }
}
