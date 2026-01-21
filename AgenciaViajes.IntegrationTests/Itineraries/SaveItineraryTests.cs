using AgenciaViajes.Domain.Entities;
using AgenciaViajes.Domain.Request;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AgenciaViajes.IntegrationTests.Itineraries
{
	public class SaveAndGetItineraryTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
	{
        [Fact]
		public async Task Should_Save_And_Return_Itinerary()
		{
            // Arrange
            ItineraryRequest request = new("Senderismo por Asturias",
				["Gijón", "Oviedo", "Cangas de Onís"])
			{
				Name = "Senderismo por Asturias",
			};

			// Act
			var id = await Itineraries.SaveItineraryAsync(request);

			// Assert servicio
			var result = await Itineraries.GetItineraryAsync(id);

			result.Should().NotBeNull();
			result!.Id.Should().Be(id);
			result.Name.Should().Be("Senderismo por Asturias");
			result.Cities.Should().HaveCount(3)
				.And.Contain(x => x.Name == "Gijón")
				.And.Contain(x => x.Name == "Oviedo")
				.And.Contain(x => x.Name == "Cangas de Onís");

			// Assert directo a Mongo
			var database = DbContext.GetDatabase("AgenciaViajesDB"); // según tu configuración
			var collection = database.GetCollection<Itinerary>("Itinerarios");

			var dbItem = await collection
				.Find(Builders<Itinerary>.Filter.Eq("Id", id))
				.FirstOrDefaultAsync();

			dbItem.Should().NotBeNull();
			dbItem.Name.Should().Be("Senderismo por Asturias");

			var cities = dbItem.Cities.Select(x => x.Name).ToList();
			cities.Should().HaveCount(3)
				  .And.Contain(["Gijón", "Oviedo", "Cangas de Onís"]);
		}

		[Fact]
		public async Task Should_Save_And_Return_Itineraries()
		{
			// Arrange
			ItineraryRequest request = new("Senderismo por Asturias",
				["Gijón", "Oviedo", "Cangas de Onís"])
			{
				Name = "Senderismo por Asturias",
			};

			// Arrange
			ItineraryRequest request2 = new("Senderismo por Galicia",
				["La Coruña", "Santiago de Compostela", "Ferrol"])
			{
				Name = "Senderismo por Galicia",
			};

			// Act
			var id = await Itineraries.SaveItineraryAsync(request);
			var id2 = await Itineraries.SaveItineraryAsync(request2);

			// Assert servicio
			var result = await Itineraries.GetItinerariesAsync();

			result.Should().NotBeNull();
			result.Count.Should().Be(3);

			// Assert directo a Mongo
			var database = DbContext.GetDatabase("AgenciaViajesDB"); // según tu configuración
			var collection = database.GetCollection<Itinerary>("Itinerarios");

			var list = await collection.FindAsync(Builders<Itinerary>.Filter.Empty);

			var items = await list.ToListAsync();
			items.Should().HaveCount(3);
			list.Should().NotBeNull();

			var dbItem = await collection
				.Find(Builders<Itinerary>.Filter.Eq("Id", id))
				.FirstOrDefaultAsync();
			var cities = dbItem.Cities.Select(x => x.Name).ToList();
			cities.Should().HaveCount(3)
				  .And.Contain(["Gijón", "Oviedo", "Cangas de Onís"]);
		}
	}
}
