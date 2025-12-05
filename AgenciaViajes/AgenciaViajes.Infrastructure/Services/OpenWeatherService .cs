using AgenciaViajes.Domain.Entities;
using AgenciaViajes.Domain.HttpClient;
using AgenciaViajes.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Json;
using System.Diagnostics.CodeAnalysis;

namespace AgenciaViajes.Infrastructure.Services;

public static partial class OpenWeatherServiceLogger
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "City '{CityName}' not found by the weather provider.")]
    public static partial void CityNotFound(this ILogger logger, string cityName);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "HTTP request to OpenWeather API failed.")]
    public static partial void HttpRequestFailed(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Unable to parse forecast timestamps from weather provider.")]
    public static partial void UnableToParseForecastTimestamps(this ILogger logger);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Forecast does not contain temperature data.")]
    public static partial void ForecastMissingTemperature(this ILogger logger);
}

public class OpenWeatherService(IHttpClientFactory httpClientFactory, ILogger<OpenWeatherService> logger, IOptions<WheatherClient> wheatherClient) : IWeatherService
{
    public async Task<WeatherData> GetWeatherAsync(string cityName, DateTime travelDate)
    {
        if (string.IsNullOrWhiteSpace(cityName))
        {
            throw new ArgumentException("City name must be provided.", nameof(cityName));
        }

        string apiKey = wheatherClient.Value.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.HttpRequestFailed(new InvalidOperationException("OpenWeather API key is not configured."));
            throw new InvalidOperationException("Weather provider API key is not configured.");
        }

        string escapedCity = Uri.EscapeDataString(cityName);
        string requestUri = $"https://api.openweathermap.org/data/2.5/forecast?q={escapedCity}&appid={apiKey}&units=metric";

        HttpClient httpClient = httpClientFactory.CreateClient(wheatherClient.Value.Name);
        // Protección por tiempo máximo de petición (evita esperar indefinidamente).
        using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));        

        HttpResponseMessage response;
        try
        {
            response = await httpClient.GetAsync(new Uri(requestUri), HttpCompletionOption.ResponseHeadersRead, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex) when (cts.IsCancellationRequested)
        {
            logger.HttpRequestFailed(ex);
            throw new TimeoutException("Timeout contacting the weather service.", ex);
        }
        catch (HttpRequestException ex)
        {
            logger.HttpRequestFailed(ex);
            throw new InvalidOperationException("Error contacting the weather service.", ex);
        }
        catch (Exception ex)
        {
            logger.HttpRequestFailed(ex);
            throw;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.CityNotFound(cityName);
            throw new KeyNotFoundException($"City '{cityName}' not found by the weather provider.");
        }

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            logger.HttpRequestFailed(ex);
            throw new InvalidOperationException("Weather provider returned a non-success status code.", ex);
        }

        OpenWeatherResponse? data;
        try
        {
            data = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>(cancellationToken: cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex) when (cts.IsCancellationRequested)
        {
            logger.HttpRequestFailed(ex);
            throw new TimeoutException("Timeout reading weather response.", ex);
        }
        catch (Exception ex)
        {
            logger.HttpRequestFailed(ex);
            throw new InvalidOperationException("Failed to deserialize weather response.", ex);
        }

        if (data?.List == null || data.List.Count == 0)
        {
            throw new InvalidOperationException("Weather service returned no forecast data.");
        }

        // Parse forecast timestamps as numeric Unix seconds (Dt) and choose the best match for today (UTC).
        DateTime targetDate = DateTime.UtcNow; // cálculo para el día de hoy (UTC)

        List<(Forecast Forecast, DateTime DateTime)> forecasts = data.List
            .Select(f =>
            {
                DateTime dt = default;

                // Se espera que `Dt` sea un número (segundos Unix). Intentar leerlo de forma segura.
                object? dtValue = GetPropertyValue(f, "Dt");
                try
                {
                    if (dtValue is double d)
                    {
                        dt = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(d)).UtcDateTime;
                    }
                    else if (dtValue is long l)
                    {
                        dt = DateTimeOffset.FromUnixTimeSeconds(l).UtcDateTime;
                    }
                    else if (dtValue is int i)
                    {
                        dt = DateTimeOffset.FromUnixTimeSeconds(i).UtcDateTime;
                    }
                    else if (dtValue is string s && long.TryParse(s, out var parsed))
                    {
                        dt = DateTimeOffset.FromUnixTimeSeconds(parsed).UtcDateTime;
                    }
                }
                catch (InvalidCastException)
                {
                    dt = default;
                }
                catch (FormatException)
                {
                    dt = default;
                }
                catch (OverflowException)
                {
                    dt = default;
                }
                // Elimina el catch general y relanza cualquier excepción inesperada.
                return (Forecast: f, DateTime: dt);
            })
            .Where(t => t.DateTime != default)
            .ToList();

        // Seleccionar previsión del mismo día (hoy UTC) o la más cercana en segundos
        List<(Forecast Forecast, DateTime DateTime)> sameDay = forecasts.Where(t => t.DateTime.Date == targetDate.Date).ToList();
        (Forecast Forecast, DateTime DateTime) best = (sameDay.Count > 0 ? sameDay : forecasts)
            .OrderBy(t => Math.Abs((t.DateTime - targetDate).TotalSeconds))
            .First();

        Forecast forecast = best.Forecast;

        if (forecast?.Main == null)
        {
            logger.ForecastMissingTemperature();
            throw new InvalidOperationException("Forecast does not contain temperature data.");
        }

        double temperature;
        try
        {
            temperature = forecast.Main.Temp;
        }
        catch (InvalidCastException ex)
        {
            logger.ForecastMissingTemperature();
            throw new InvalidOperationException("Unable to read temperature from forecast.", ex);
        }
        catch (NullReferenceException ex)
        {
            logger.ForecastMissingTemperature();
            throw new InvalidOperationException("Unable to read temperature from forecast.", ex);
        }

        bool rain = false;
        try
        {
            if (forecast.Weather is not null)
            {
                rain = forecast.Weather
                    .Select(w => w.Main)
                    .Any(main => !string.IsNullOrEmpty(main) && main.Contains("rain", StringComparison.OrdinalIgnoreCase));
            }
        }
        catch (InvalidCastException)
        {
            rain = false;
        }
        catch (NullReferenceException)
        {
            rain = false;
        }

        return new WeatherData
        {
            Temperature = temperature,
            Rain = rain
        };

        static object? GetPropertyValue(object obj, string propertyName)
        {
            // Helper para acceder a propiedades deserializadas sin depender del tipo concreto.
            var prop = obj.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return prop?.GetValue(obj);
        }
    }
}
