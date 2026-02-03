using Prometheus;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Services
// --------------------
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Kestrel (container-friendly)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

// --------------------
// Middleware
// --------------------

// Prometheus HTTP metrics (request count, duration, status codes)
app.UseHttpMetrics();

// Swagger
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API v1");
        c.RoutePrefix = "swagger";
    });
}

// --------------------
// Endpoints
// --------------------

// Welcome endpoint
app.MapGet("/", () => new
{
    Message = "Welcome to the Weather App!",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow
})
.WithName("GetWelcome")
.WithTags("General");

// Weather endpoint
app.MapGet("/weather", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    var forecast = Enumerable.Range(1, 5)
        .Select(index => new
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Length)]
        })
        .Select(temp => new
        {
            temp.Date,
            temp.TemperatureC,
            TemperatureF = 32 + (int)(temp.TemperatureC / 0.5556),
            temp.Summary
        });

    return forecast;
})
.WithName("GetWeatherForecast")
.WithTags("Weather");

// Health check (Kubernetes-ready)
app.MapHealthChecks("/health")
   .WithTags("Health");

// Prometheus metrics endpoint
app.MapMetrics();

// --------------------
app.Run();
