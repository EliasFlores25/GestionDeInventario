using OpenQA.Selenium.Chrome;
using Xunit;

namespace GestionDeInventario.Tests.Selenium
{
    public class SeleniumVerificarRespuestaAplicacion
    {
        [Fact]
        public void AplicacionRespondeASolicitud()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");

            using var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl("http://localhost:5000");

            Assert.False(driver.PageSource.Contains("404"));
        }
    }
}
