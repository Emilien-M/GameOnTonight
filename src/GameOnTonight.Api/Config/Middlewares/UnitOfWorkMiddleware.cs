using GameOnTonight.Api.Config.Attributes;
using GameOnTonight.Domain.Repositories;

namespace GameOnTonight.Api.Config.Middlewares;

/// <summary>
/// Middleware that automatically applies the UnitOfWork at the end of each HTTP request.
/// </summary>
public class UnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UnitOfWorkMiddleware> _logger;

    public UnitOfWorkMiddleware(RequestDelegate next, ILogger<UnitOfWorkMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode is >= 200 and < 300)
            {
                if (ShouldApplyUnitOfWork(context))
                {
                    await unitOfWork.SaveChangesAsync();
                    _logger.LogDebug("UnitOfWork applied successfully for request {Method} {Path}", 
                        context.Request.Method, context.Request.Path);
                }
                else
                {
                    _logger.LogDebug("UnitOfWork skipped for request {Method} {Path} based on attribute settings", 
                        context.Request.Method, context.Request.Path);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during UnitOfWork middleware execution");
            throw;
        }
    }

    private bool ShouldApplyUnitOfWork(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            return !IsReadOnlyHttpMethod(context.Request.Method);
        }

        var unitOfWorkAttribute = endpoint.Metadata.GetMetadata<UnitOfWorkAttribute>();
        if (unitOfWorkAttribute != null)
        {
            return unitOfWorkAttribute.Enabled;
        }

        return !IsReadOnlyHttpMethod(context.Request.Method);
    }

    private bool IsReadOnlyHttpMethod(string httpMethod)
    {
        return HttpMethods.IsGet(httpMethod) || 
               HttpMethods.IsHead(httpMethod) || 
               HttpMethods.IsOptions(httpMethod) || 
               HttpMethods.IsTrace(httpMethod);
    }
}

/// <summary>
/// Extension to facilitate adding the middleware to the HTTP pipeline.
/// </summary>
public static class UnitOfWorkMiddlewareExtensions
{
    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UnitOfWorkMiddleware>();
    }
}
