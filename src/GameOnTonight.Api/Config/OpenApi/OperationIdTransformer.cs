using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GameOnTonight.Api.Config.OpenApi;

/// <summary>
/// Transformateur de document OpenAPI qui attribue des operationId uniques basés sur le contrôleur et l'action
/// </summary>
public class OperationIdTransformer : IOpenApiDocumentTransformer
{
    /// <summary>
    /// Transforme le document OpenAPI pour attribuer des operationId uniques à toutes les opérations
    /// </summary>
    /// <param name="document">Le document OpenAPI à modifier</param>
    /// <param name="context">Le contexte de transformation associé au document</param>
    /// <param name="cancellationToken">Le jeton d'annulation à utiliser</param>
    /// <returns>La tâche représentant l'opération asynchrone</returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // Parcourir chaque chemin et chaque opération dans le document
        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                // Rechercher la description API correspondante
                var apiDescription = FindApiDescription(context, path.Key, operation.Key.ToString());
                
                if (apiDescription != null && apiDescription.ActionDescriptor is ControllerActionDescriptor controllerAction)
                {
                    // Obtenir le nom du contrôleur (sans le suffixe "Controller")
                    string controllerName = controllerAction.ControllerName;
                    
                    // Obtenir le nom de l'action
                    string actionName = controllerAction.ActionName;

                    // Récupérer le verbe HTTP (GET, POST, etc.)
                    string httpMethod = operation.Key.ToString();

                    // Extraire les informations de route pour les paramètres
                    string routeParams = "";
                    if (path.Key.Contains("{"))
                    {
                        // Si la route contient des paramètres, les extraire et les ajouter à l'operationId
                        var parts = path.Key.Split('/');
                        var paramParts = parts.Where(p => p.StartsWith("{") && p.EndsWith("}")).ToList();
                        
                        if (paramParts.Any())
                        {
                            foreach (var part in paramParts)
                            {
                                // Extraire le nom du paramètre des accolades et le mettre en majuscule
                                string paramName = part.Trim('{', '}');
                                routeParams += $"By{char.ToUpper(paramName[0])}{paramName[1..]}";
                            }
                        }
                    }

                    // Construire un operationId unique basé sur le contrôleur, l'action, le verbe HTTP et les paramètres de route
                    // Format : [Controller][Action|HttpMethod][ByParam]
                    string operationId;
                    
                    // Si le nom de l'action est standard (Get, Post, Put, Delete)
                    // et correspond au verbe HTTP, utiliser uniquement le contrôleur + verbe
                    if (actionName.Equals(httpMethod, StringComparison.OrdinalIgnoreCase) ||
                        (httpMethod == "Get" && actionName == "GetAll") ||
                        (httpMethod == "Post" && actionName == "Create") ||
                        (httpMethod == "Put" && actionName == "Update") ||
                        (httpMethod == "Delete" && actionName == "Delete"))
                    {
                        operationId = $"{controllerName}{httpMethod}{routeParams}";
                    }
                    else
                    {
                        // Sinon, utiliser le nom de l'action personnalisé
                        operationId = $"{controllerName}{actionName}{routeParams}";
                    }
                    
                    // Assigner l'operationId à l'opération
                    operation.Value.OperationId = operationId;
                }
            }
        }

        // Retourner une tâche complétée car cette méthode n'effectue pas d'opérations asynchrones
        return Task.CompletedTask;
    }

    /// <summary>
    /// Trouve la description de l'API correspondant à un chemin et une méthode HTTP donnés
    /// </summary>
    private Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription? FindApiDescription(
        OpenApiDocumentTransformerContext context, 
        string path, 
        string httpMethod)
    {
        // Normaliser le chemin pour la comparaison
        var normalizedPath = path.TrimEnd('/').ToLowerInvariant();
        if (!normalizedPath.StartsWith("/"))
            normalizedPath = "/" + normalizedPath;

        // Normaliser la méthode HTTP pour la comparaison
        var normalizedMethod = httpMethod.ToUpperInvariant();
        
        // Parcourir tous les groupes de description API
        foreach (var group in context.DescriptionGroups)
        {
            foreach (var description in group.Items)
            {
                // Normaliser le chemin de la description API
                var descriptionPath = description.RelativePath?.ToLowerInvariant();
                if (descriptionPath != null && !descriptionPath.StartsWith("/"))
                    descriptionPath = "/" + descriptionPath;
                
                // Vérifier si le chemin et la méthode correspondent
                if (descriptionPath != null && 
                    descriptionPath.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase) &&
                    description.HttpMethod != null &&
                    description.HttpMethod.Equals(normalizedMethod, StringComparison.OrdinalIgnoreCase))
                {
                    return description;
                }
            }
        }

        return null;
    }
}
