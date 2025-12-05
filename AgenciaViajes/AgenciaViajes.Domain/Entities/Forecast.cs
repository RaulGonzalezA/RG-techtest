using System.Collections.ObjectModel;

namespace AgenciaViajes.Domain.Entities;

public class Forecast
{
    public Forecast(double dt, Main main, Collection<Weather> weather)
    {
        Dt = dt;
        Main = main;
        Weather = weather;
    }

    public double Dt { get; set; }
    public Main Main { get; set; }
    public Collection<Weather> Weather { get; internal set; }

}
