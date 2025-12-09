using AgenciaViajes.Application.Interfaces;
using AgenciaViajes.Domain.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace AgenciaViajes.Host.Controllers
{
	internal static class Agency
    {
		public static void MapAgencyEndpoints(this WebApplication app)
		{
            app.MapPost("/itineraries", async (
            ItineraryRequest itineraryDto,
            IItineraryService itineraryService) =>
            {
                var id = await itineraryService.SaveItineraryAsync(itineraryDto).ConfigureAwait(false);
                return Results.CreatedAtRoute("GetItinerary", new { id }, new
                {
                    Id = id
                });
            })
				.WithName("Post Itinerary")
			    .WithTags("Itineraries")
			    .WithMetadata(new SwaggerOperationAttribute(summary: "Post Itinerary", description: "Adds new itinerary to the app"))
			    .Produces(StatusCodes.Status201Created);

			app.MapGet("/itineraries/{id}", async (
                string id,
                IItineraryService itineraryService) =>
            {
                var itinerary = await itineraryService.GetItineraryAsync(id).ConfigureAwait(false);
                if (itinerary == null)
                    return Results.NotFound(new { Message = "Itinerary not found." });
                return Results.Ok(itinerary);
            })
				.WithName("GetItinerary")
				.WithTags("Itineraries")
				.WithMetadata(new SwaggerOperationAttribute(summary: "Get Itinerary", description: "Gets an itinerary from the app"))
				.Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            app.MapGet("/itineraries", async (
                IItineraryService itineraryService) =>
            {
                var itinerary = await itineraryService.GetItinerariesAsync().ConfigureAwait(false);
                if (itinerary == null)
                    return Results.NotFound(new { Message = "Itineraries not found." });
                return Results.Ok(itinerary);
            })
                .WithName("GetItineraries")
                .WithTags("Itineraries")
                .WithMetadata(new SwaggerOperationAttribute(summary: "Get Itineraries", description: "Gets all the itineraries from the app"))
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
        }
    }
}
