var builder = WebApplication.CreateBuilder(args);

builder
    .AddObservability()
    .AddPrometheusMetrics();

builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationAuthorizationPolicies()
    .AddOpenApiDocumentation()
    .AddGatewayCors(builder.Configuration)
    .AddGatewayForwardedHeaders(builder.Configuration)
    .AddGatewayRateLimiting()
    .AddGatewayHealthChecks()
    .AddGatewayReverseProxy(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

var app = builder.Build();

app.UseRequestCorrelation();
app.UseSerilogRequestLogging();
app.UseExceptionHandling();
app.UseGatewayForwardedHeaders();
app.UseGatewaySecurity();
app.UseCors(CorsConfiguration.PolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseSecurityHeaders();

app.UsePrometheusMetrics();
app.MapGatewayEndpoints();
app.UseOpenApiDocumentation();
app.MapReverseProxy();

app.Run();
