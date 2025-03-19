using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog(
    (context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration) // Read from appsettings.json
            .Enrich.WithProperty("ApplicationName", "Search Api") // Add custom properties
            .WriteTo.Console(); // Enable logging in the console
    }
);

// Add services to the container.

builder.Services.AddControllers();

// Register Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "My Search API",
            Version = "v1",
            Description = "Search API with Core 9",
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable Swagger & Swagger UI
if (app.Environment.IsDevelopment()) // Show only in Development mode
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Search API v1");
    });
}

app.UseSerilogRequestLogging(); // Logs HTTP requests

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
