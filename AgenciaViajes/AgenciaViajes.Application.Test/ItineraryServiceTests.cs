using AgenciaViajes.Application.Services;
using AgenciaViajes.Domain.Entities;
using AgenciaViajes.Domain.Request;
using AgenciaViajes.Infrastructure.Interfaces;
using FluentAssertions;
using NSubstitute;
using System.Collections.ObjectModel;

namespace AgenciaViajes.Application.Test
{
#pragma warning disable CA1515
    public sealed class ItineraryServiceTests
    {
        private readonly ItineraryService _service;
        private readonly IItineraryRepository _repo;
        private readonly IWeatherService _weatherService;
        private static readonly string[] ExpectedRecommendations = new[] { "Abrigo", "Sudadera", "Paraguas" };

        public ItineraryServiceTests()
        {
            _repo = Substitute.For<IItineraryRepository>();
            _weatherService = Substitute.For<IWeatherService>();
            _service = new ItineraryService(_repo, _weatherService);
        }

        [Fact]
        public async Task SaveItineraryAsyncInsertsItineraryAndReturnsId()
        {
            // Arrange
            var request = (ItineraryRequest)System.Activator.CreateInstance(typeof(ItineraryRequest), new object[] { "MyTrip", new Collection<string> { "CityA", "CityB" } })!;

            // Act
            var id = await _service.SaveItineraryAsync(request).ConfigureAwait(false);

            // Assert
            id.Should().NotBeNullOrEmpty();
            await _repo.Received(1).InsertAsync(Arg.Is<Itinerary>(it => it.Name == "MyTrip" && it.Cities.Count == 2 && it.Cities[0].Name == "CityA")).ConfigureAwait(false);
        }

        [Fact]
        public async Task GetItineraryAsyncReturnsNullWhenNotFound()
        {
            // Arrange            
            _repo.FindByIdAsync(Arg.Any<string>()).Returns(Task.FromResult<Itinerary?>(null)!);            

            // Act
            var result = await _service.GetItineraryAsync("missing-id").ConfigureAwait(false);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetItineraryAsyncReturnsResponseWithCityWeatherAndRecommendations()
        {
            // Arrange
            var cities = new Collection<City>
            {
                new(Guid.NewGuid().ToString(),"CityA", System.DateTime.Today),
                new(Guid.NewGuid().ToString(),"CityB", System.DateTime.Today.AddDays(1))
            };

            var itinerary = new Itinerary("id-1", "TripName", cities);
            _repo.FindByIdAsync("id-1").Returns(Task.FromResult(itinerary));

            _weatherService.GetWeatherAsync(Arg.Is<string>(s => s == "CityA"), Arg.Is<System.DateTime>(d => d.Date == System.DateTime.Today))
                .Returns(Task.FromResult(new WeatherData { Temperature = 15, Rain = false }));

            _weatherService.GetWeatherAsync(Arg.Is<string>(s => s == "CityB"), Arg.Is<System.DateTime>(d => d.Date == System.DateTime.Today.AddDays(1)))
                .Returns(Task.FromResult(new WeatherData { Temperature = 5, Rain = true }));

            // Act
            var result = await _service.GetItineraryAsync("id-1").ConfigureAwait(false);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("id-1");
            result.Name.Should().Be("TripName");
            result.Cities.Should().HaveCount(2);

            var first = result.Cities[0];
            first.Name.Should().Be("CityA");
            first.Temperature.Should().Be(15);
            first.Rain.Should().BeFalse();
            first.Recommendations.Should().ContainSingle(r => r == "Sudadera");

            var second = result.Cities[1];
            second.Name.Should().Be("CityB");
            second.Temperature.Should().Be(5);
            second.Rain.Should().BeTrue();
            second.Recommendations.Should().Contain(ExpectedRecommendations);

            await _weatherService.Received(1).GetWeatherAsync("CityA", Arg.Is<System.DateTime>(d => d.Date == System.DateTime.Today)).ConfigureAwait(false);
            await _weatherService.Received(1).GetWeatherAsync("CityB", Arg.Is<System.DateTime>(d => d.Date == System.DateTime.Today.AddDays(1))).ConfigureAwait(false);
        }
    }
}
