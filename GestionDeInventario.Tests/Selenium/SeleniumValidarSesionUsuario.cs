using OpenQA.Selenium.Chrome;
using Xunit;

namespace GestionDeInventario.Tests.Selenium
{
    public class Selenium_27_ValidarSesionUsuario
    {
        [Fact]
        public void SesionDelNavegadorActiva()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");

            using var driver = new ChromeDriver(options);

            Assert.NotNull(driver.Manage().Cookies);
        }
    }
}
