using Backend.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Backend.Middleware;

/// <summary>
/// Middleware global de tratamento de exceções.
/// Converte exceções em ProblemDetails (RFC 7807).
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var response = new ProblemDetails
        {
            Instance = context.Request.Path
        };

        switch (exception)
        {
            case ProductNotFoundException pnfEx:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Status = StatusCodes.Status404NotFound;
                response.Title = "Produto não encontrado";
                response.Detail = pnfEx.Message;
                response.Type = "https://api.example.com/errors/product-not-found";
                break;

            case InsufficientStockException issEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = StatusCodes.Status400BadRequest;
                response.Title = "Estoque insuficiente";
                response.Detail = issEx.Message;
                response.Type = "https://api.example.com/errors/insufficient-stock";
                var extensions = new Dictionary<string, object?>
                {
                    { "code", "ESTOQUE_INSUFICIENTE" },
                    { "quantidadeDisponivel", issEx.QuantidadeDisponivel },
                    { "quantidadeSolicitada", issEx.QuantidadeSolicitada }
                };
                response.Extensions = extensions;
                break;

            case InvalidMovementTypeException imtEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Status = StatusCodes.Status400BadRequest;
                response.Title = "Tipo de movimentação inválido";
                response.Detail = imtEx.Message;
                response.Type = "https://api.example.com/errors/invalid-movement-type";
                break;

            case ConcurrencyException cEx:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Status = StatusCodes.Status409Conflict;
                response.Title = "Conflito de concorrência";
                response.Detail = cEx.Message;
                response.Type = "https://api.example.com/errors/concurrency-conflict";
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Status = StatusCodes.Status500InternalServerError;
                response.Title = "Erro interno do servidor";
                response.Detail = "Um erro inesperado ocorreu. Por favor, tente novamente mais tarde.";
                response.Type = "https://api.example.com/errors/internal-server-error";
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// RFC 7807 ProblemDetails.
/// </summary>
public class ProblemDetails
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public Dictionary<string, object?>? Extensions { get; set; }
}

/// <summary>
/// Extensão para registrar o middleware no pipeline.
/// </summary>
public static class GlobalExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
