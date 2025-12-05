namespace AgenciaViajes.Domain.Entities;

public class City
{
    public City(string name, DateTime date)
    {
        Name = name;
        Date = date;
    }

    public string Name { get; set; }
    public DateTime Date { get; set; }        
}
