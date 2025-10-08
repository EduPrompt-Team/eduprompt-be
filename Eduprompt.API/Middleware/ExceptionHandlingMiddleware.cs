using System.Net;
using System.Text.Json;

namespace Eduprompt.API.Middleware;

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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An internal server error occurred. Please contact support.";

        switch (exception)
        {
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized; // 401
                message = exception.Message;
                break;
                
            case ForbiddenException:
                statusCode = HttpStatusCode.Forbidden; // 403
                message = exception.Message;
                break;
                
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound; // 404
                message = exception.Message;
                break;
                
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest; // 400
                message = exception.Message;
                break;
                
            case ArgumentException: // Includes ArgumentNullException
                statusCode = HttpStatusCode.BadRequest; // 400
                message = exception.Message;
                break;
        }

        var response = new
        {
            statusCode = (int)statusCode,
            message = message,
            timestamp = DateTime.UtcNow,
            path = context.Request.Path.ToString()
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

/// <summary>
/// Custom exception for 403 Forbidden responses
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException() : base("Access forbidden") { }
    public ForbiddenException(string message) : base(message) { }
    public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
}
