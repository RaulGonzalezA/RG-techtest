using AgenciaViajes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgenciaViajes.Infrastructure.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherAsync(string cityName, DateTime travelDate);
    }
}
