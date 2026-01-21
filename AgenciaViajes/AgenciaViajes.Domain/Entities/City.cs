namespace AgenciaViajes.Domain.Entities;

public class City : EntityBase
{
    private City() : base(string.Empty)
    {
        Name = string.Empty;
        Date = DateTime.MinValue;
    }

    public City(string id, string name, DateTime date) : base(id)
    {
        Name = name;
        Date = date;
    }

    public string Name { get; private set; }
    public DateTime Date { get; private set; }        
}
