using GameOnTonight.Domain.Repositories;

namespace GameOnTonight.Api.Middlewares;

/// <summary>
/// Middleware qui applique automatiquement le UnitOfWork à la fin de chaque requête HTTP
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
            // Traitement de la requête
            await _next(context);

            // Si la requête est GET ou OPTIONS, on ne sauvegarde pas car elles sont considérées comme en lecture seule
            if (HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsOptions(context.Request.Method) || 
                HttpMethods.IsHead(context.Request.Method) || HttpMethods.IsTrace(context.Request.Method))
            {
                return;
            }

            // Si la requête s'est terminée avec succès (statut 2xx), on applique l'UnitOfWork
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                await unitOfWork.SaveChangesAsync();
                _logger.LogDebug("UnitOfWork applied successfully for request {Method} {Path}", 
                    context.Request.Method, context.Request.Path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during UnitOfWork middleware execution");
            throw;
        }
    }
}

/// <summary>
/// Extension pour faciliter l'ajout du middleware à la pipeline HTTP
/// </summary>
public static class UnitOfWorkMiddlewareExtensions
{
    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UnitOfWorkMiddleware>();
    }
}
