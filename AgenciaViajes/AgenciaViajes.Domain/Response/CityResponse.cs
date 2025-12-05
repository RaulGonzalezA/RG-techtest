using AgenciaViajes.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AgenciaViajes.Domain.Response
{
    public record CityResponse
    {
        public CityResponse(string name, DateTime date, double temperature, bool rain, Collection<string> recommendations)
        {
            Name = name;
            Date = date;
            Temperature = temperature;
            Rain = rain;
            Recommendations = recommendations;
        }

        public string Name { get; set; }
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public bool Rain { get; set; }
        public Collection<string> Recommendations { get; internal set; }
    }
}
