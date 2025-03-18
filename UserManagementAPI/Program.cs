var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Add Swagger services
builder.Services.AddLogging(); // Add logging services

var app = builder.Build();

var users = new Dictionary<string, User>
{
    { "Alice", new User { Username = "Alice", Age = 30 } },
    { "Bob", new User { Username = "Bob", Age = 40 } },
    { "Charlie", new User { Username = "Charlie", Age = 50 } }
};

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API V1");
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>(); // Use the error-handling middleware
app.UseMiddleware<LoggingMiddleware>(); // Use the logging middleware
app.UseMiddleware<TokenValidationMiddleware>(); // Use the token validation middleware

app.MapGet("/users", () => users.Values);

app.MapGet("/users/{username}", (string username) =>
{
    if (string.IsNullOrEmpty(username))
    {
        return Results.BadRequest(new { error = "Username cannot be null or empty." });
    }

    if (users.TryGetValue(username, out var user))
    {
        return Results.Ok(user);
    }

    return Results.NotFound(new { error = "User not found." });
});

app.MapPost("/users", (User user) =>
{
    if (string.IsNullOrEmpty(user.Username) || user.Age <= 0)
    {
        return Results.BadRequest(new { error = "Invalid user data." });
    }

    if (users.ContainsKey(user.Username))
    {
        return Results.Conflict(new { error = "User already exists." });
    }

    users[user.Username] = user;
    return Results.Created($"/users/{user.Username}", user);
});

app.MapPut("/users/{username}", (string username, User updatedUser) =>
{
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(updatedUser.Username) || updatedUser.Age <= 0)
    {
        return Results.BadRequest(new { error = "Invalid user data." });
    }

    if (!users.ContainsKey(username))
    {
        return Results.NotFound(new { error = "User not found." });
    }

    users[username] = updatedUser;
    return Results.Ok(updatedUser);
});

app.MapDelete("/users/{username}", (string username) =>
{
    if (string.IsNullOrEmpty(username))
    {
        return Results.BadRequest(new { error = "Username cannot be null or empty." });
    }

    if (!users.Remove(username))
    {
        return Results.NotFound(new { error = "User not found." });
    }

    return Results.NoContent();
});

app.Run();

public class User {
    required public string Username { get; set; }
    public int Age { get; set; }
}