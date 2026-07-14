using System.Net;
using System.Text.Json;
using GymAPI.Domain.Exceptions;

namespace GymAPI.Api.Middleware;

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
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            DomainException ex => (HttpStatusCode.BadRequest, ex.Message),
            ExerciseNotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
            KeyNotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access."),
            ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),
            InvalidOperationException ex => (HttpStatusCode.Conflict, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = new
            {
                message,
                statusCode = (int)statusCode
            }
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
