using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace GameOnTonight.Api.Config;

public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Entrez votre token JWT"
        };

        document.Components ??= new OpenApiComponents();
        if (document.Components.SecuritySchemes == null)
        {
            document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>();
        }
        document.Components.SecuritySchemes.Add("Bearer", securityScheme);

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer"),
                []
            }
        };

        document.Security ??= [];
        document.Security.Add(securityRequirement);

        return Task.CompletedTask;
    }
}