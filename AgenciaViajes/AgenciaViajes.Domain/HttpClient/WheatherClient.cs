namespace AgenciaViajes.Domain.HttpClient;

public record WheatherClient
{
    public Uri BaseUrl { get; set; } = new Uri("https://api.openweathermap.org/data/2.5/");
    public string ApiKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}