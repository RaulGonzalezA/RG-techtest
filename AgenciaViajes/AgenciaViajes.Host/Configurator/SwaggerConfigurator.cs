namespace AgenciaViajes.Host.Configurator;

/// <summary>
/// Configurador de Swagger para la documentación de la API.
/// </summary>
internal static class SwaggerConfigurator
{
    /// <summary>
    /// Configura Swagger para la documentación de la API.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    internal static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
            {
                Title = "Agencia de Viajes API",
                Version = "v1",
                Description = "API para la gestión de una agencia de viajes."
            });

            string xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
        return services;
    }
}
