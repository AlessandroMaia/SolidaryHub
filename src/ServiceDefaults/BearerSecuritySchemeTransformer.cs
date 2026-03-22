using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace ServiceDefaults;

public sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider provider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var schemes = await provider.GetAllSchemesAsync();

        if (!schemes.Any(authScheme => authScheme.Name == JwtBearerDefaults.AuthenticationScheme))
            return;

        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            [JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token"
            }
        };

        document.Components ??= new();
        document.Components.SecuritySchemes = securitySchemes;

        if (document.Paths is null)
            return;

        foreach (var path in document.Paths.Values)
        {
            if (path.Operations is null)
                continue;

            foreach (var operation in path.Operations)
            {
                if (operation.Value is null)
                    return;

                operation.Value.Security ??= [];
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
                });
            }
        }
    }
}
