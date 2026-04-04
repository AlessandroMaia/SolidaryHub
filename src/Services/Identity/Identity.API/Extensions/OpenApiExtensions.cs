namespace Identity.API.Extensions;

public static class OpenApiExtensions
{
    private const string OpenApiRoutePath = "/openapi/v1.json";

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
            app.MapOpenApi();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "Documentação disponível em: http://localhost:{Port}{OpenApiPath}",
                    app.Configuration["ASPNETCORE_HTTP_PORTS"] ?? "5001",
                    OpenApiRoutePath);
            }
        }

        return app;
    }
}
