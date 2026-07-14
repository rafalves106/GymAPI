namespace GymAPI.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString();
        context.Items["RequestId"] = requestId;

        _logger.LogInformation("[{RequestId}] {Method} {Path} started",
            requestId, context.Request.Method, context.Request.Path);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation("[{RequestId}] {Method} {Path} completed with {StatusCode} in {ElapsedMs}ms",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}
