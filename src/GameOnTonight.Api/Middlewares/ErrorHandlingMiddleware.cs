using System.Net;
using System.Text.Json;
using GameOnTonight.Api.Models;
using GameOnTonight.Domain.Exceptions;

namespace GameOnTonight.Api.Middlewares;

/// <summary>
/// Middleware pour la gestion globale des exceptions dans l'API
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException domainException)
        {
            await HandleDomainExceptionAsync(context, domainException);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleDomainExceptionAsync(HttpContext context, DomainException domainException)
    {
        _logger.LogError(domainException, "Erreur de validation du domaine: {Message}", domainException.Message);

        // Les exceptions du domaine sont considérées comme des BadRequest (400)
        var statusCode = HttpStatusCode.BadRequest;
        var errorTitle = "Erreur de validation du domaine";
            
        // Grouper les erreurs par propriété
        var errors = domainException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                group => string.IsNullOrEmpty(group.Key) ? "error" : group.Key, 
                group => group.Select(e => e.Message).ToArray()
            );

        await WriteErrorResponseAsync(context, statusCode, errorTitle, errors);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Une erreur est survenue: {Message}", exception.Message);

        // Toute autre exception est traitée comme une erreur serveur (500)
        var statusCode = HttpStatusCode.InternalServerError;
        var errorTitle = _environment.IsDevelopment() 
            ? exception.Message 
            : "Une erreur interne est survenue.";
                
        var errors = new Dictionary<string, string[]>
        {
            { "error", new[] { exception.Message } }
        };
            
        // En développement, on ajoute le détail de la stacktrace
        if (_environment.IsDevelopment())
        {
            errors.Add("stackTrace", new[] { exception.StackTrace ?? "No stack trace available" });
        }

        await WriteErrorResponseAsync(context, statusCode, errorTitle, errors);
    }
    
    /// <summary>
    /// Écrit la réponse d'erreur standardisée dans le flux HTTP
    /// </summary>
    private async Task WriteErrorResponseAsync(
        HttpContext context, 
        HttpStatusCode statusCode, 
        string errorTitle, 
        Dictionary<string, string[]> errors)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)statusCode;
        
        var errorResponse = new ErrorResponse(errorTitle, statusCode, errors);

        var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(result);
    }
}

/// <summary>
/// Extension pour faciliter l'enregistrement du middleware
/// </summary>
public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
