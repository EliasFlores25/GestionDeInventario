using OpenQA.Selenium.Chrome;
using Xunit;

namespace GestionDeInventario.Tests.Selenium
{
    public class SeleniumSimulacionInicioSesion
    {
        [Fact]
        public void SimularFlujoInicioSesion()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");

            using var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://localhost:5000/Login");

            Assert.NotNull(driver.PageSource);
        }
    }
}

