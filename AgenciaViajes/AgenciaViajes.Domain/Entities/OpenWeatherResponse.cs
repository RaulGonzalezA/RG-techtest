using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AgenciaViajes.Domain.Entities;

public class OpenWeatherResponse
{
    public OpenWeatherResponse(Collection<Forecast> list)
    {
        List = list;
    }

    public Collection<Forecast> List { get; internal set; }
}
