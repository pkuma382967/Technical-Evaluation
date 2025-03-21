using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SearchAPI.Data;
using SearchAPI.Middleware;
using SearchAPI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog(
    (context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration) // Read from appsettings.json
            .WriteTo.Console(); // Enable logging in the console
    }
);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SearchAppDBConnection"))
);

// Add services to the container.

builder.Services.AddControllers();

// Register Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Search API", Version = "v1" });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Enter 'Bearer' followed by your JWT. Example: Bearer eyJhbGciOiJIUzI1NiIsInR...",
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                    Scheme = "Bearer",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            },
        }
    );
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthService, AuthService>();

// authentication
// Add authentication


builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(jwtSettings["Key"])
                ),
            };
        }
    );

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.


// Middleware for error handling
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseMiddleware<RateLimitingMiddleware>();

// Enable Swagger & Swagger UI
if (app.Environment.IsDevelopment()) // Show only in Development mode
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(); // Logs HTTP requests

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
