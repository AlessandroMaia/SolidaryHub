namespace ApiGateway.Configuration;

public static class HealthChecksConfiguration
{
    public static IServiceCollection AddGatewayHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}
