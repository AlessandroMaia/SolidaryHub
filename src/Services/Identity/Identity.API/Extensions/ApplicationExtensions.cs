namespace Identity.API.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediator(assembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<>), typeof(LoggingBehavior<>));
        services.AddScoped(typeof(IPipelineBehavior<>), typeof(ValidationBehavior<>));

        return services;
    }

    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDb")
            ?? throw new InvalidOperationException("IdentityDb connection string not found");

        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddNpgSql(
                connectionString,
                name: "identity-db",
                tags: ["db", "postgres", "ready"]);

        return services;
    }
}
