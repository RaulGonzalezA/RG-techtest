using System.Collections.ObjectModel;

namespace AgenciaViajes.Domain.Entities;

public class Itinerary
{
    public Itinerary(string id, string name, Collection<City> cities)
    {
        Id = id;
        Name = name;
        Cities = cities;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public Collection<City> Cities { get; internal set; }
}
