namespace Campaign.API.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediator(assembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<>), typeof(LoggingBehavior<>));
        services.AddScoped(typeof(IPipelineBehavior<>), typeof(ValidationBehavior<>));
        services.AddScoped(typeof(IPipelineBehavior<>), typeof(TransactionBehavior<>));

        services.AddScoped<ICampaignIntegrationEventService, CampaignIntegrationEventService>();

        return services;
    }

    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CampaignDb")
            ?? throw new InvalidOperationException("CampaignDb connection string not found");

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddNpgSql(
                connectionString,
                name: "campaign-db",
                tags: ["db", "postgres", "ready"]);

        return services;
    }
}