using AgenciaViajes.Application.Interfaces;
using AgenciaViajes.Domain.Entities;
using AgenciaViajes.Domain.Request;
using AgenciaViajes.Domain.Response;
using AgenciaViajes.Infrastructure.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

namespace AgenciaViajes.Application.Services
{
    public class ItineraryService(
        IItineraryRepository itineraryRepository,
        IWeatherService weatherService
        ) : IItineraryService
    {
        public async Task<string> SaveItineraryAsync(ItineraryRequest itineraryDto)
        {
            ArgumentNullException.ThrowIfNull(itineraryDto);
            if (itineraryDto.Cities == null)
                throw new ArgumentException("Cities collection must be provided.", nameof(itineraryDto));
            var cities = itineraryDto.Cities.Select((city, index) => new City(Guid.NewGuid().ToString(), city, DateTime.Today.AddDays(index))).ToList();
            var itinerary = new Itinerary(Guid.NewGuid().ToString(), itineraryDto.Name, new Collection<City>(cities));

            await itineraryRepository.InsertAsync(itinerary).ConfigureAwait(false);
            return itinerary.Id;
        }

        public async Task<ItineraryResponse?> GetItineraryAsync(string id)
        {
            Itinerary? itinerary = await itineraryRepository.FindByIdAsync(id).ConfigureAwait(false);
            if (itinerary == null)
                return null;

            Collection<CityResponse> cities = new();
            foreach (var city in itinerary.Cities)
            {
                var weather = await weatherService.GetWeatherAsync(city.Name, city.Date).ConfigureAwait(false);
                CityResponse cityResponse = new(city.Name, city.Date, weather.Temperature, weather.Rain, GetRecommendations(weather.Temperature, weather.Rain));
                cities.Add(cityResponse);
            }

            return new ItineraryResponse(itinerary.Id, itinerary.Name, cities);
        }

        public async Task<List<Itinerary>> GetItinerariesAsync()
        {
            return await itineraryRepository.FindAllAsync().ConfigureAwait(false);
        }

        private static Collection<string> GetRecommendations(double temperature, bool rain)
        {
            var recommendations = new Collection<string>();
            if (temperature < 10)
                recommendations.Add("Abrigo");
            if (temperature < 20)
                recommendations.Add("Sudadera");
            if (rain)
                recommendations.Add("Paraguas");
            return recommendations;
        }
    }
}
