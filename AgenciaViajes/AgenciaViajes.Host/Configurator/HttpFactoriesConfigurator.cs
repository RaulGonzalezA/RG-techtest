using AgenciaViajes.Domain.HttpClient;
using Microsoft.Extensions.DependencyInjection;

namespace AgenciaViajes.Host.Configurator
{
    internal static class HttpFactoriesConfigurator
    {
        private const string WeatherClientName = "WheatherClient";

        public static IServiceCollection ConfigureHttpFactories(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            WheatherClient wheatherClient = new WheatherClient();
            configurationManager.GetSection(WeatherClientName).Bind(wheatherClient);

            services.Configure<WheatherClient>(configurationManager.GetSection(WeatherClientName));

            services.AddHttpClient(wheatherClient.Name, client =>
            {
                client.BaseAddress = wheatherClient.BaseUrl;
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            return services;
        }
    }
}
