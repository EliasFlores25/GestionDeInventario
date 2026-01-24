using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace GestionDeInventario.Tests.Selenium
{
    public class SeleniumAbrirNavegador
    {
        [Fact]
        public void AbrirChromeSinError()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");

            using var driver = new ChromeDriver(options);

            Assert.NotNull(driver);
        }
    }
}
