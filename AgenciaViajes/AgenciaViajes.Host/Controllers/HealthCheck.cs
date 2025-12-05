using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace AgenciaViajes.Host.Controllers;

internal static class HealthCheck
{
    private static readonly Action<ILogger, string, Exception?> _logHealthy =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(1, nameof(_logHealthy)),
            "Healthy: {Services}");

    private static readonly Action<ILogger, string, Exception?> _logUnhealthy =
        LoggerMessage.Define<string>(
            LogLevel.Information,
            new EventId(2, nameof(_logUnhealthy)),
            "Unhealthy: {Services}");

    public static void MapHealthCheckEndpoints(this WebApplication app)
    {
		app.MapGet("/health", async (HealthCheckService healthCheckService) => {
            HealthReport healthReport = await healthCheckService.CheckHealthAsync().ConfigureAwait(false);
			ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

			string serializedReport = JsonSerializer.Serialize(healthReport);

            if (healthReport.Status == HealthStatus.Healthy)
            {
                _logHealthy(logger, serializedReport, null);
                return Results.Ok(new { status = "Healthy", details = healthReport.Entries });
            }
            else
            {
                _logUnhealthy(logger, serializedReport, null);
                return Results.Problem(new ProblemDetails(){ Status = 503, Detail  = "The service is Unhealthy", Title = "Unhealthy" });
            }
        })
            .WithName("HealthCheck")
            .WithTags("Health")
            .WithMetadata(new SwaggerOperationAttribute(summary: "Healthcheck for the api", description:"Gets the healthchek"))
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status503ServiceUnavailable);
    }
}