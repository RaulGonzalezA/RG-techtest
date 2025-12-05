using AgenciaViajes.Application.Interfaces;
using AgenciaViajes.Application.Services;
using AgenciaViajes.Domain.Consts;
using AgenciaViajes.Host.Configurator;
using AgenciaViajes.Host.Controllers;
using AgenciaViajes.Host.ExceptionHandling;
using AgenciaViajes.Infrastructure.Interfaces;
using AgenciaViajes.Infrastructure.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using RepositoryMongoDb.Settings;
using Serilog;
using System.Diagnostics;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

HttpFactoriesConfigurator.ConfigureHttpFactories(builder.Services, builder.Configuration);
DbConfigurator.ConfigureMongoDb(builder.Services, builder.Configuration);
SwaggerConfigurator.ConfigureSwagger(builder.Services);

builder.Services.AddScoped<IWeatherService, OpenWeatherService>();
builder.Services.AddScoped<IItineraryService, ItineraryService>();

builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = context => { 
    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.TraceId.ToString() ?? Activity.Current?.TraceId.ToString());
});

MongoDBSettings agenciaSettings = new();
builder.Configuration.GetSection(DatabaseSettings.MONGOAGENCIA).Bind(agenciaSettings);
builder.Services
    .AddSingleton(new MongoClient(agenciaSettings.ConnectionString))
    .AddHealthChecks()
    .AddMongoDb(databaseNameFactory: sp => agenciaSettings.DatabaseName);

builder.Host.UseSerilog((context, options) => 
    options.ReadFrom.Configuration(context.Configuration));

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

HealthCheck.MapHealthCheckEndpoints(app);
Agency.MapAgencyEndpoints(app);

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

await app.RunAsync().ConfigureAwait(false);
