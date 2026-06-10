using System.Net;
using System.Text.Json;

namespace UsersApi.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred while processing {Method} {Path}",
            context.Request.Method, context.Request.Path);

        var (statusCode, type) = MapException(exception);

        var response = new
        {
            message = exception.Message,
            date = DateTime.UtcNow,
            statusCode = (int)statusCode,
            type,
            path = $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}",
            traceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode StatusCode, string Type) MapException(Exception exception)
    {
        return exception switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, "ValidationError"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "NotFound"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            InvalidOperationException => (HttpStatusCode.Conflict, "Conflict"),
            _ => (HttpStatusCode.InternalServerError, "InternalServerError")
        };
    }
}
