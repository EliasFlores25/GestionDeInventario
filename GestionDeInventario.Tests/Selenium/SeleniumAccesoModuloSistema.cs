using OpenQA.Selenium.Chrome;
using Xunit;

namespace GestionDeInventario.Tests.Selenium
{
    public class SeleniumAccesoModuloSistema
    {
        [Fact]
        public void AccederModuloDepartamento()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");

            using var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://localhost:5000/Departamento");

            Assert.NotNull(driver.Url);
        }
    }
}
