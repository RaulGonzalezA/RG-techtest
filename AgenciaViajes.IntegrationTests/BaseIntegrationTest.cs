using AgenciaViajes.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AgenciaViajes.IntegrationTests
{
	public abstract class BaseIntegrationTest
	: IClassFixture<IntegrationTestWebAppFactory>,
	  IDisposable
	{
		private readonly IServiceScope _scope;
		protected readonly IItineraryService Itineraries;
		protected readonly IMongoClient DbContext;

		protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
		{
			_scope = factory.Services.CreateScope();

			Itineraries = _scope.ServiceProvider.GetRequiredService<IItineraryService>();

			DbContext = _scope.ServiceProvider
				.GetRequiredService<IMongoClient>();
		}

		public void Dispose()
		{
			// Scope contiene dependencias scoped (handlers, servicios, etc.)
			_scope?.Dispose();

			// NO disponer el IMongoClient aquí: está registrado como Singleton en el factory
			// y es compartido por todos los tests. Si lo dispones, otros tests fallan en paralelo.
		}
	}
}