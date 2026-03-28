using Campaign.API.Application.IntegrationEvents.EventHandling;
using EventBusRabbitMQ;

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
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<>), typeof(LoggingBehavior<>));
        services.AddScoped(typeof(IPipelineBehavior<>), typeof(ValidationBehavior<>));
        services.AddScoped(typeof(IPipelineBehavior<>), typeof(TransactionBehavior<>));
        services.AddScoped(typeof(IPipelineBehavior<>), typeof(IdempotencyBehavior<>));

        services.AddScoped<CampaignIntegrationEventService>();
        services.AddScoped<ICampaignIntegrationEventService>(sp => sp.GetRequiredService<CampaignIntegrationEventService>());
        services.AddScoped<ITransactionEventPublisher>(sp => sp.GetRequiredService<CampaignIntegrationEventService>());

        return services;
    }

    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CampaignDb")
            ?? throw new InvalidOperationException("A connection string 'CampaignDb' não foi encontrada.");

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddNpgSql(
                connectionString,
                name: "campaign-db",
                tags: ["db", "postgres", "ready"]);

        return services;
    }

    public static IHostApplicationBuilder ConfigureEventBus(this IHostApplicationBuilder builder)
    {
        builder.AddRabbitMqEventBus(builder.Configuration.GetConnectionString("RabbitMQ")!, "campaign-service")
            .AddSubscription<DonationIntentProcessedIntegrationEvent, DonationIntentProcessedIntegrationEventHandler>()
            .AddSubscription<DonationIntentFailedIntegrationEvent, DonationIntentFailedIntegrationEventHandler>();


        return builder;
    }
}
