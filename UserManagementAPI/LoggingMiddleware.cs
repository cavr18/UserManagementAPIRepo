public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log the request method and path
        _logger.LogInformation("HTTP {Method} {Path} requested", context.Request.Method, context.Request.Path);

        // Call the next middleware in the pipeline
        await _next(context);

        // Log the response status code
        _logger.LogInformation("HTTP {Method} {Path} responded with {StatusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
    }
}