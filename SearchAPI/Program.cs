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
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.


// Middleware for error handling
app.UseMiddleware<ErrorHandlingMiddleware>();

// Enable Swagger & Swagger UI
if (app.Environment.IsDevelopment()) // Show only in Development mode
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(); // Logs HTTP requests

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
