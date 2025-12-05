using AgenciaViajes.Domain.Entities;
using System.Collections.ObjectModel;

namespace AgenciaViajes.Domain.Response
{
    public record ItineraryResponse
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public Collection<CityResponse> Cities { get; internal set; }

        public ItineraryResponse(string id, string name, Collection<CityResponse> cities)
        {
            Id = id;
            Name = name;
            Cities = cities;
        }
    }
}