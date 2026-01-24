using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace GestionDeInventario.Tests.Selenium
{
    public class SeleniumRecargarPagina
    {
        [Fact]
        public void RecargarPagina()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");

            using var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://localhost:5000");
            driver.Navigate().Refresh();

            Assert.True(true);
        }
    }
}
