using AgenciaViajes.Domain.Consts;
using AgenciaViajes.Infrastructure.Interfaces;
using AgenciaViajes.Infrastructure.Repository;
using MongoDB.Driver;
using RepositoryMongoDb.Settings;

namespace AgenciaViajes.Host.Configurator;

internal static class DbConfigurator
{
    public static IServiceCollection ConfigureMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        MongoDBSettings agenciaSettings = new();        
        configuration.GetSection(DatabaseSettings.MONGOAGENCIA).Bind(agenciaSettings);
        services.Configure<MongoDBSettings>(configuration.GetSection(DatabaseSettings.MONGOAGENCIA));
		services.AddSingleton<IMongoClient>(new MongoClient(agenciaSettings.ConnectionString));
		services.AddSingleton<IItineraryRepository, ItineraryRepository>();
        return services;
    }
}
