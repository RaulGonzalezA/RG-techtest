using AgenciaViajes.Domain.Entities;
using System.Collections.ObjectModel;

namespace AgenciaViajes.Domain.Request;

public record ItineraryRequest
{
    public ItineraryRequest(string name, Collection<string> cities)
    {
        Name = name;
        Cities = cities;
    }

    public required string Name { get; set; }
    public Collection<string> Cities { get; internal set; }
}
