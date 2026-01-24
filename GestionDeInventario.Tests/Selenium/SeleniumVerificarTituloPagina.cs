using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace GestionDeInventario.Tests.Selenium
{
    public class SeleniumVerificarTituloPagina
    {
        [Fact]
        public void ObtenerTituloPagina()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");

            using var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://localhost:5000");

            var titulo = driver.Title;

            Assert.NotNull(titulo);
        }
    }
}
