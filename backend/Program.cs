using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Backend.Services;
using Backend.Repositories;
using Backend.Models;
using Backend.Infrastructure;
using Backend.Mapping;
using Backend.Validators;
using Backend.Middleware;
using FluentValidation;
using System;
using System.Threading;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services
builder.Services.AddControllers(options =>
    {
        // Run our FluentValidation action filter globally so query/body/route DTOs are validated.
        options.Filters.Add<Backend.Middleware.FluentValidationActionFilter>();
    })
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Customize the automatic 400 response so JSON parse / model binding errors
// produce a clearer, normalized ValidationProblemDetails payload.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var modelState = context.ModelState;
        var errors = new Dictionary<string, string[]>();

        foreach (var kvp in modelState)
        {
            var key = kvp.Key ?? string.Empty;
            // Normalize keys like "$.codigoProduto" -> "codigoProduto"
            var normalizedKey = key.StartsWith("$.") ? key.Substring(2) : key;

            var messages = kvp.Value.Errors
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? (e.Exception?.Message ?? "Invalid value.") : e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToArray();

            if (messages.Length > 0)
            {
                // Use a sensible fallback key name when the key is empty
                errors[string.IsNullOrWhiteSpace(normalizedKey) ? "request" : normalizedKey] = messages;
            }
        }

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        };

        return new BadRequestObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register health checks so MapHealthChecks can be used later
builder.Services.AddHealthChecks();

// EF Core DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=.;Database=TargetGupy;Trusted_Connection=true;Encrypt=false;";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repositories
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

// Services
builder.Services.AddScoped<IMovimentacaoParser, MovimentacaoParser>();
builder.Services.AddScoped<IMovimentacaoCalculationService, MovimentacaoCalculationService>();
builder.Services.AddScoped<IMovimentacaoService, MovimentacaoService>();
builder.Services.AddScoped<ICommissionService, CommissionService>();
builder.Services.AddScoped<IFeesService, FeesService>();

// Configuration options
// CommissionOptions removed in favor of simple constants; no DI registration required.

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<MovimentacaoCreateRequestValidator>();
// Note: we run FluentValidation validators via a global action filter (FluentValidationActionFilter)
// Register our filter so it can be resolved from DI
builder.Services.AddScoped<Backend.Middleware.FluentValidationActionFilter>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Global exception handling middleware
app.UseGlobalExceptionHandling();

app.UseCors("_myAllowSpecificOrigins");

app.UseSwagger();
app.UseSwaggerUI();

// Attempt to apply EF Core migrations on startup with retry logic.
// This helps when SQL Server container is still starting.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    const int maxRetries = 10;
    var delay = TimeSpan.FromSeconds(5);
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation("Applying database migrations (attempt {Attempt}/{Max})...", attempt, maxRetries);
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not apply migrations on attempt {Attempt}/{Max}.", attempt, maxRetries);
            if (attempt == maxRetries)
            {
                logger.LogError(ex, "Exceeded max retries applying migrations. Application startup will continue but DB may not be available.");
                break;
            }
            Thread.Sleep(delay);
        }
    }
}

app.UseRouting();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Expose Program class for integration tests (WebApplicationFactory)
public partial class Program { }