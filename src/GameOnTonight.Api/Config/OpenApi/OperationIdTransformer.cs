using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace GameOnTonight.Api.Config.OpenApi;

/// <summary>
/// OpenAPI document transformer that assigns unique operationIds based on the controller and action.
/// </summary>
internal sealed class OperationIdTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        foreach (var path in document.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                var apiDescription = FindApiDescription(context, path.Key, operation.Key.ToString());
                
                if (apiDescription != null && apiDescription.ActionDescriptor is ControllerActionDescriptor controllerAction)
                {
                    string controllerName = controllerAction.ControllerName;
                    string actionName = controllerAction.ActionName;
                    string httpMethod = operation.Key.ToString();

                    string routeParams = "";
                    if (path.Key.Contains("{"))
                    {
                        var parts = path.Key.Split('/');
                        var paramParts = parts.Where(p => p.StartsWith("{") && p.EndsWith("}")).ToList();
                        
                        if (paramParts.Any())
                        {
                            foreach (var part in paramParts)
                            {
                                string paramName = part.Trim('{', '}');
                                routeParams += $"By{char.ToUpper(paramName[0])}{paramName[1..]}";
                            }
                        }
                    }

                    string operationId;
                    
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
                        operationId = $"{controllerName}{actionName}{routeParams}";
                    }
                    
                    operation.Value.OperationId = operationId;
                }
            }
        }

        return Task.CompletedTask;
    }

    private Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription? FindApiDescription(
        OpenApiDocumentTransformerContext context, 
        string path, 
        string httpMethod)
    {
        var normalizedPath = path.TrimEnd('/').ToLowerInvariant();
        if (!normalizedPath.StartsWith("/"))
            normalizedPath = "/" + normalizedPath;

        var normalizedMethod = httpMethod.ToUpperInvariant();
        
        foreach (var group in context.DescriptionGroups)
        {
            foreach (var description in group.Items)
            {
                var descriptionPath = description.RelativePath?.ToLowerInvariant();
                if (descriptionPath != null && !descriptionPath.StartsWith("/"))
                    descriptionPath = "/" + descriptionPath;
                
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
