namespace ApiGateway.Configuration;

public static class ForwardedHeadersConfiguration
{
    public static IServiceCollection AddGatewayForwardedHeaders(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var trustAllProxies = configuration.GetValue("ForwardedHeaders:TrustAllProxies", false);

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto |
                ForwardedHeaders.XForwardedHost;

            if (trustAllProxies)
            {
                options.KnownIPNetworks.Clear();
                options.KnownProxies.Clear();
            }
        });

        return services;
    }

    public static IApplicationBuilder UseGatewayForwardedHeaders(this IApplicationBuilder app)
    {
        app.UseForwardedHeaders();
        return app;
    }
}
