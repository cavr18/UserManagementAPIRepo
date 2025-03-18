public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        ErrorResponse response;
        switch (ex)
        {
            case ArgumentNullException argNullEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse { Error = "Bad Request", Details = argNullEx.Message };
                break;
            case ArgumentException argEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse { Error = "Bad Request", Details = argEx.Message };
                break;
            case KeyNotFoundException keyNotFoundEx:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new ErrorResponse { Error = "Not Found", Details = keyNotFoundEx.Message };
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ErrorResponse { Error = "Internal server error.", Details = ex.Message };
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse {
    public string? Error { get; set; }
    public string? Details { get; set; }
}