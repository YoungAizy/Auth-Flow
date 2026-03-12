using System.Linq;
using Auth.Controllers;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Auth.Tests.Controllers
{
    public class ControllersTests
    {
        [Fact]
        public void Registration_Get_ReturnsTwoValues()
        {
            var controller = new RegistrationController();

            var result = controller.Get().ToArray();

            Assert.Equal(2, result.Length);
            Assert.Equal("value1", result[0]);
            Assert.Equal("value2", result[1]);
        }

        [Fact]
        public void Registration_GetById_ReturnsValue()
        {
            var controller = new RegistrationController();

            var result = controller.Get(5);

            Assert.Equal("value", result);
        }

        [Fact]
        public void WeatherForecast_Get_ReturnsFiveForecasts()
        {
            var logger = NullLogger<WeatherForecastController>.Instance;
            var controller = new WeatherForecastController(logger);

            var result = controller.Get().ToArray();

            Assert.Equal(5, result.Length);
        }
    }
}
