using AgenciaViajes.Domain.Entities;
using AgenciaViajes.Domain.HttpClient;
using AgenciaViajes.Infrastructure.Services;
using FluentAssertions;
using NSubstitute;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;

namespace AgenciaViajes.Infrastructure.Test
{
#pragma warning disable CA1515
    public class OpenWeatherServiceTests
    {
        [Fact]
        public async Task GetWeatherAsyncThrowsArgumentExceptionWhenCityNameEmpty()
        {
            // Arrange
            var httpFactory = Substitute.For<IHttpClientFactory>();
            var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<OpenWeatherService>>();
            var options = Microsoft.Extensions.Options.Options.Create(new WheatherClient { ApiKey = "key", Name = "openweather" });
            var service = new OpenWeatherService(httpFactory, logger, options);

            // Act
            Func<Task> act = async () => await service.GetWeatherAsync("", DateTime.Today).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task GetWeatherAsyncThrowsKeyNotFoundWhenCityNotFound()
        {
            // Arrange
            var handler = new DelegatingHandlerStub((request, ct) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                return Task.FromResult(response);
            });
            var client = new HttpClient(handler);

            var httpFactory = Substitute.For<IHttpClientFactory>();
            httpFactory.CreateClient(Arg.Any<string>()).Returns(client);

            var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<OpenWeatherService>>();
            var options = Microsoft.Extensions.Options.Options.Create(new WheatherClient { ApiKey = "key", Name = "openweather" });
            var service = new OpenWeatherService(httpFactory, logger, options);

            // Act
            Func<Task> act = async () => await service.GetWeatherAsync("UnknownCity", DateTime.Today).ConfigureAwait(false);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().ConfigureAwait(false);
            logger.Received().CityNotFound("UnknownCity");
        }

        [Fact]
        public async Task GetWeatherAsyncReturnsWeatherDataWhenForecastAvailable()
        {
            // Arrange
            var sample = new OpenWeatherResponse(new Collection<Forecast>
                {
                    new Forecast (DateTime.Today.Ticks, new Main { Temp = 12 }, new Collection<Weather> { new Weather ("Clear") } ) ,
                    new Forecast (DateTime.Today.AddDays(1).Ticks, new Main { Temp = 5 }, new Collection < Weather > { new Weather ("Rain" ) })
                }
            );

            var handler = new DelegatingHandlerStub((request, ct) =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(sample)
                };
                return Task.FromResult(response);
            });
            var client = new HttpClient(handler);

            var httpFactory = Substitute.For<IHttpClientFactory>();
            httpFactory.CreateClient(Arg.Any<string>()).Returns(client);

            var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<OpenWeatherService>>();
            var options = Microsoft.Extensions.Options.Options.Create(new WheatherClient { ApiKey = "key", Name = "openweather" });
            var service = new OpenWeatherService(httpFactory, logger, options);

            // Act
            var resultToday = await service.GetWeatherAsync("CityA", DateTime.Today).ConfigureAwait(false)  ;
            var resultTomorrow = await service.GetWeatherAsync("CityA", DateTime.Today.AddDays(1)).ConfigureAwait(false);

            // Assert
            resultToday.Temperature.Should().Be(12);
            resultToday.Rain.Should().BeFalse();

            resultTomorrow.Temperature.Should().Be(5);
            resultTomorrow.Rain.Should().BeTrue();
        }
    }

    internal sealed class DelegatingHandlerStub : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

        public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handler(request, cancellationToken);
        }
    }
}
