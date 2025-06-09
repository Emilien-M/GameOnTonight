using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics; // Required for IExceptionHandler

namespace GameOnTonight.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment()
                ? new ApiExceptionResponse(ex.Message, ex.StackTrace?.ToString())
                : new ApiExceptionResponse("An unexpected error occurred. Please try again later.");

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var jsonResponse = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}

// Simple DTO for the error response
public record ApiExceptionResponse(string Message, string? Details = null);
