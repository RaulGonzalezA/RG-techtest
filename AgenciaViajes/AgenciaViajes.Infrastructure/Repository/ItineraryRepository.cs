using AgenciaViajes.Domain.Entities;
using AgenciaViajes.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RepositoryMongoDb.Repository;
using RepositoryMongoDb.Settings;
using System.Linq.Expressions;

namespace AgenciaViajes.Infrastructure.Repository
{
    public class ItineraryRepository : MongoDbRepository<Itinerary, string>, IItineraryRepository
    {
#pragma warning disable CA1062 // Validar argumentos de métodos públicos
        public ItineraryRepository(IMongoClient mongoClient, IOptions<MongoDBSettings> mongoDbSettings) : base(mongoClient, mongoDbSettings.Value.DatabaseName, mongoDbSettings.Value.Collections["Itinerarios"])
#pragma warning restore CA1062 // Validar argumentos de métodos públicos
        {
            ArgumentNullException.ThrowIfNull(mongoDbSettings);
        }

        protected override Expression<Func<Itinerary, bool>> GetIdFilterExpression(string id)
        {
            return Itinerary => Itinerary.Id == id;
        }
    }
}
