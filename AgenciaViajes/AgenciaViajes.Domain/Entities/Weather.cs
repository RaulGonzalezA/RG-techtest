namespace AgenciaViajes.Domain.Entities;

public class Weather
{
    public Weather(string main)
    {
        Main = main;
    }

    public string Main { get; set; }
}
