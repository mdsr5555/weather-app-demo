using Swashbuckle.AspNetCore.SwaggerUI;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container

builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// Configure Kestrel to listen on port 8080 (required for containers)

builder.WebHost.ConfigureKestrel(options =>

{

options.ListenAnyIP(8080);

});


var app = builder.Build();


// Configure the HTTP request pipeline

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())

{

    app.UseSwagger();

    app.UseSwaggerUI(c =>

    {

    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API v1");

    c.RoutePrefix = "swagger";

    });

}


// Define API endpoints

app.MapGet("/", () => new

{

Message = "Welcome to the Weather App!",

Version = "1.0.0",

Environment = app.Environment.EnvironmentName,

Timestamp = DateTime.UtcNow

})

.WithName("GetWelcome")

.WithTags("General");


app.MapGet("/weather", () =>

{

var summaries = new[]

{

"Freezing", "Bracing", "Chilly", "Cool", "Mild",

"Warm", "Balmy", "Hot", "Sweltering", "Scorching"

};




var forecast = Enumerable.Range(1, 5).Select(index => new
{
    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)) ,
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


// Health check endpoint (required for Kubernetes)

app.MapHealthChecks("/health")

.WithTags("Health");


app.Run();





// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

// app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast");

// app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
