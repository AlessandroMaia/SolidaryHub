namespace Campaign.API.Extensions;

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
                    Title = "Solidary Hub - Campaign API",
                    Version = "v1",
                    Description = "Microserviço de controle de campanhas e doações",
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
                    .WithTitle("Campaign API")
                    .WithTheme(ScalarTheme.Solarized)
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });

            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Documentação disponível em: https://localhost:{Port}/scalar/v1",
                        app.Configuration["ASPNETCORE_HTTPS_PORT"] ?? "5002");
            }

        }

        return app;
    }
}