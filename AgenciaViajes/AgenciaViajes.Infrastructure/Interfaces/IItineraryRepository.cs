using AgenciaViajes.Domain.Entities;
using RepositoryMongoDb.Repository;

namespace AgenciaViajes.Infrastructure.Interfaces
{
    public interface IItineraryRepository : IMongoDbRepository<Itinerary, string>
    {
    }
}
