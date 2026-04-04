namespace ApiGateway.Configuration;

public static class CorsConfiguration
{
    public const string PolicyName = "GatewayCors";

    public static IServiceCollection AddGatewayCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (origins.Length > 0)
                {
                    policy.WithOrigins(origins);
                }

                policy.AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
