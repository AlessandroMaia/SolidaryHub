namespace Identity.API.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(static (document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = "Solidary Hub - Identity API",
                    Version = "v1",
                    Description = "Microserviço de Autenticação e Autorização",
                    Contact = new()
                    {
                        Name = "SHTeam",
                        Email = "alessandro@solidaryhub.org.com"
                    }
                };

                return Task.CompletedTask;
            });

            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }

    public static IApplicationBuilder UseOpenApiDocumentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("Identity API")
                    .WithTheme(ScalarTheme.Solarized)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });

            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Documentação disponível em: https://localhost:{Port}/scalar/v1",
                        app.Configuration["ASPNETCORE_HTTPS_PORT"] ?? "5001");
            }

        }

        return app;
    }
}

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider provider)
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
