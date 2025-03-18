public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenValidationMiddleware> _logger;

    public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var tokenValues))
        {
            _logger.LogWarning("Authorization header missing.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        var token = tokenValues.FirstOrDefault();
        if (string.IsNullOrEmpty(token) || !ValidateToken(token))
        {
            _logger.LogWarning("Invalid token.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
            return;
        }

        await _next(context);
    }

    private bool ValidateToken(string token)
    {
        // Implement your token validation logic here
        // For example, you can check if the token is in a list of valid tokens
        // or validate it using a JWT library
        return token == "valid-token"; // Replace this with your actual validation logic
    }
}