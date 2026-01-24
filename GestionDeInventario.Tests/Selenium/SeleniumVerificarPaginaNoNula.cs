using OpenQA.Selenium.Chrome;
using Xunit;

namespace GestionDeInventario.Tests.Selenium
{
    public class SeleniumVerificarPaginaNoNula
    {
        [Fact]
        public void PaginaNoEsNula()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");

            using var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://localhost:5000");

            Assert.NotNull(driver.PageSource);
        }
    }
}
