using System.Net;
using System.Text.Json;
using FluentValidation;
using GameOnTonight.Domain.Exceptions;

namespace GameOnTonight.Api.Middlewares;

/// <summary>
/// Middleware that handles exceptions and converts them to appropriate HTTP responses.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlingMiddleware> logger,
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
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain validation error occurred");
            await HandleDomainExceptionAsync(context, ex);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error occurred");
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt");
            await HandleUnauthorizedExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred");
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private async Task HandleDomainExceptionAsync(HttpContext context, DomainException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/problem+json";

        var errors = exception.Errors.ToDictionary(
            e => e.Name,
            e => new[] { e.Message });

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            title = "Domain validation error",
            status = (int)HttpStatusCode.BadRequest,
            errors
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/problem+json";

        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            title = "Validation error",
            status = (int)HttpStatusCode.BadRequest,
            errors
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleUnauthorizedExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            title = "Unauthorized",
            status = (int)HttpStatusCode.Unauthorized,
            detail = exception.Message
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleGenericExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            title = "Internal server error",
            status = (int)HttpStatusCode.InternalServerError,
            detail = _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred"
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
