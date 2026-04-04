namespace ApiGateway.Configuration;

public static class OpenApiConfiguration
{
    private const string GatewayOpenApiPath = "/openapi/v1.json";
    private const string GatewayDocsOpenApiPath = "/docs/openapi/gateway.json";
    private const string DocsPath = "/docs";
    private const string GatewayDocsPath = "/docs/gateway";
    private const string IdentityDocsPath = "/docs/identity";
    private const string CampaignDocsPath = "/docs/campaign";
    private const string DocsOpenApiPattern = "/docs/openapi/{documentName}.json";

    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(static (document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = "Portal do Desenvolvedor Solidarity Hub",
                    Version = "v1",
                    Description = "Portal centralizado de documentação e gateway YARP seguro para as APIs do Solidarity Hub.",
                    Contact = new()
                    {
                        Name = "SHTeam",
                        Email = "alessandro@solidarityhub.org.com"
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
            app.MapOpenApi()
                .AllowAnonymous()
                .ExcludeFromDescription();

            app.MapGet(GatewayDocsOpenApiPath, () => Results.Redirect(GatewayOpenApiPath, permanent: false))
                .AllowAnonymous()
                .ExcludeFromDescription();

            app.MapGet(GatewayDocsPath, () => Results.Redirect(DocsPath, permanent: false))
                .AllowAnonymous()
                .ExcludeFromDescription();

            app.MapGet(IdentityDocsPath, () => Results.Redirect(DocsPath, permanent: false))
                .AllowAnonymous()
                .ExcludeFromDescription();

            app.MapGet(CampaignDocsPath, () => Results.Redirect(DocsPath, permanent: false))
                .AllowAnonymous()
                .ExcludeFromDescription();

            app.MapScalarApiReference(DocsPath, (options, httpContext) =>
            {
                var origin = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

                options
                    .AddDocument("gateway", "Gateway", isDefault: true)
                    .AddDocument("identity", "Identity API")
                    .AddDocument("campaign", "Campaign API")
                    .WithOpenApiRoutePattern($"{origin}{DocsOpenApiPattern}")
                    .WithTitle("Portal do Desenvolvedor Solidarity Hub")
                    .WithTheme(ScalarTheme.Solarized)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            })
            .AllowAnonymous()
            .ExcludeFromDescription();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Documentação disponível em: http://localhost:{Port}{DocsPath}",
                    app.Configuration["ASPNETCORE_HTTP_PORTS"] ?? "5000",
                    DocsPath);
            }
        }

        return app;
    }
}
