using GameOnTonight.Api.Config.Attributes;
using GameOnTonight.Domain.Repositories;

namespace GameOnTonight.Api.Config.Middlewares;

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

            // Si la requête s'est terminée avec succès (statut 2xx)
            if (context.Response.StatusCode is >= 200 and < 300)
            {
                // Vérifier si on doit appliquer l'UnitOfWork
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

    /// <summary>
    /// Détermine si l'UnitOfWork doit être appliqué en fonction des attributs et de la méthode HTTP
    /// </summary>
    /// <param name="context">Le contexte HTTP de la requête</param>
    /// <returns>Vrai si l'UnitOfWork doit être appliqué, faux sinon</returns>
    private bool ShouldApplyUnitOfWork(HttpContext context)
    {
        // Récupérer l'endpoint correspondant à la requête
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            // Si pas d'endpoint, on se base sur la méthode HTTP par défaut
            return !IsReadOnlyHttpMethod(context.Request.Method);
        }

        // Vérifier si l'attribut UnitOfWork est présent sur le endpoint
        var unitOfWorkAttribute = endpoint.Metadata.GetMetadata<UnitOfWorkAttribute>();
        if (unitOfWorkAttribute != null)
        {
            // Si l'attribut est présent, on applique sa configuration
            return unitOfWorkAttribute.Enabled;
        }

        // Par défaut, on applique l'UnitOfWork uniquement pour les méthodes non-lecture seule (POST, PUT, DELETE, etc.)
        return !IsReadOnlyHttpMethod(context.Request.Method);
    }

    /// <summary>
    /// Vérifie si la méthode HTTP est en lecture seule (GET, HEAD, OPTIONS, TRACE)
    /// </summary>
    /// <param name="httpMethod">La méthode HTTP à vérifier</param>
    /// <returns>Vrai si la méthode est en lecture seule, faux sinon</returns>
    private bool IsReadOnlyHttpMethod(string httpMethod)
    {
        return HttpMethods.IsGet(httpMethod) || 
               HttpMethods.IsHead(httpMethod) || 
               HttpMethods.IsOptions(httpMethod) || 
               HttpMethods.IsTrace(httpMethod);
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
