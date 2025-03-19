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

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) { }
app.UseSerilogRequestLogging(); // Logs HTTP requests

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
