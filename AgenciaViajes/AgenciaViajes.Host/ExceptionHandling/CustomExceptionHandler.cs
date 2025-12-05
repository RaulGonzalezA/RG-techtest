using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AgenciaViajes.Host.ExceptionHandling;

internal static partial class CustomExceptionHandlerLogMessages
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "An error occurred while processing the request: {Message}")]
    public static partial void LogProcessingError(this ILogger logger, Exception exception, string message);
}

internal sealed class CustomExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int status = exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
        httpContext.Response.StatusCode = status;

        logger.LogProcessingError(exception, exception.Message);

        ProblemDetails problemDetails = new()
        {
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
            Status = status,
            Title = exception.Message,
            Detail = exception.StackTrace,
            Type = "Error"
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        }).ConfigureAwait(false);
    }
}
