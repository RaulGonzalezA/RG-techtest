using AgenciaViajes.Domain.Request;
using AgenciaViajes.Domain.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgenciaViajes.Application.Interfaces
{
    public interface IItineraryService
    {
        Task<string> SaveItineraryAsync(ItineraryRequest itineraryDto);
        Task<ItineraryResponse?> GetItineraryAsync(string id);
    }
}
