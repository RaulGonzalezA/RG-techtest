using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace AgenciaViajes.IntegrationTests
{
	public class IntegrationTestWebAppFactory
	: WebApplicationFactory<Program>,
	  IAsyncLifetime
	{
		private readonly MongoDbContainer _dbContainer = new MongoDbBuilder("mongo:7.0")
			.WithPortBinding(27017, true)
			.WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "admin")
			.WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "Pass1234")
			.Build();

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				// 🔸 1. Eliminamos IMongoClient registrado originalmente por Host/Infra
				var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IMongoClient));
				if (descriptor != null)
				{
					services.Remove(descriptor);
				}

				// 🔸 2. Registramos el nuevo para tests (real mongo)
				var urlBuilder = new MongoUrlBuilder(_dbContainer.GetConnectionString())
				{
					Username = "admin",
					Password = "Pass1234",
					AuthenticationSource = "admin"
				};

				var client = new MongoClient(urlBuilder.ToMongoUrl());

				services.AddSingleton<IMongoClient>(client);

				// 🔸 3. (Opcional) Seed inicial
				using var sp = services.BuildServiceProvider();
				using var scope = sp.CreateScope();

				var clientMongo = scope.ServiceProvider.GetRequiredService<IMongoClient>();

				var db = clientMongo.GetDatabase("AgenciaViajesDB"); // puedes parametrizar nombre

				// por ejemplo si quieres crear colección
				db.CreateCollection("Itinerarios");
			});
		}

		public Task InitializeAsync()
		{
			return _dbContainer.StartAsync();
		}

		public new Task DisposeAsync()
		{
			return _dbContainer.StopAsync();
		}
	}
}