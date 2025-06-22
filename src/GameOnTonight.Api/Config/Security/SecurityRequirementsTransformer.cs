using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace GameOnTonight.Api.Config.Security;

public class SecurityRequirementsTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        if (document.Components?.SecuritySchemes == null || !document.Components.SecuritySchemes.ContainsKey("Bearer"))
            return Task.CompletedTask;

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        };

        // Créer une map des chemins et méthodes HTTP pour faciliter la recherche
        var protectedEndpoints = new Dictionary<string, HashSet<string>>();
        foreach (var group in context.DescriptionGroups)
        {
            foreach (var apiDescription in group.Items)
            {
                if (!IsProtectedEndpoint(apiDescription)) 
                    continue;
                
                var path = "/" + apiDescription.RelativePath?.TrimStart('/');
                var method = apiDescription.HttpMethod?.ToUpper();

                if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(method)) 
                    continue;
                    
                if (!protectedEndpoints.TryGetValue(path, out var methods))
                {
                    methods = new HashSet<string>();
                    protectedEndpoints[path] = methods;
                }
                methods.Add(method);
            }
        }

        // Appliquer les exigences de sécurité aux opérations dans le document OpenAPI
        foreach (var path in document.Paths)
        {
            var normalizedPath = path.Key;
            if (!protectedEndpoints.TryGetValue(normalizedPath, out var methods))
                continue;
            
            foreach (var operation in path.Value.Operations)
            {
                var method = operation.Key.ToString().ToUpper();
                if (!methods.Contains(method)) 
                    continue;
                operation.Value.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Value.Security.Add(securityRequirement);
            }
        }

        return Task.CompletedTask;
    }

    private static bool IsProtectedEndpoint(ApiDescription apiDescription)
    {
        if (apiDescription.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
            return false;

        // Vérifier si l'action ou le contrôleur a un attribut [Authorize]
        var authorizeAttributes = actionDescriptor.MethodInfo.GetCustomAttributes<AuthorizeAttribute>(true)
            .Concat(actionDescriptor.ControllerTypeInfo.GetCustomAttributes<AuthorizeAttribute>(true));

        // Vérifier si l'action a un attribut [AllowAnonymous] qui annulerait l'autorisation
        var allowAnonymousAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(true);

        return authorizeAttributes.Any() && allowAnonymousAttribute == null;
    }
}