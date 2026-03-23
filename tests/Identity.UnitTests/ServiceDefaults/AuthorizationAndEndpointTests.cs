using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Identity.UnitTests.ServiceDefaults;

public sealed class AuthorizationAndEndpointTests
{
    [Fact]
    public async Task AddApplicationAuthorizationPolicies_ShouldRegisterPolicies()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorizationCore();
        services.AddApplicationAuthorizationPolicies();
        await using var provider = services.BuildServiceProvider();

        var policyProvider = provider.GetRequiredService<IAuthorizationPolicyProvider>();
        var managerPolicy = await policyProvider.GetPolicyAsync(AuthorizationPolicies.RequireManagerUser);
        var donorPolicy = await policyProvider.GetPolicyAsync(AuthorizationPolicies.RequireDonorUser);
        var appUserPolicy = await policyProvider.GetPolicyAsync(AuthorizationPolicies.RequireManagerOrDonorUser);

        managerPolicy.Should().NotBeNull();
        managerPolicy!.Requirements.Should().Contain(r => r is RolesAuthorizationRequirement);
        donorPolicy.Should().NotBeNull();
        appUserPolicy.Should().NotBeNull();
    }

    [Fact]
    public async Task EndpointAuthorizationExtensions_And_DefaultEndpoints_ShouldAttachMetadata()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddAuthorization();
        builder.Services.AddHealthChecks()
            .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddCheck("ready", () => HealthCheckResult.Healthy());
        var app = builder.Build();

        app.MapGet("/manager", () => "ok").RequireManagerAccess();
        app.MapGet("/donor", () => "ok").RequireDonorAccess();
        app.MapGet("/app-user", () => "ok").RequireApplicationUserAccess();
        app.MapDefaultEndpoints();

        var endpoints = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(ds => ds.Endpoints)
            .ToList();

        endpoints.Count(e => e.DisplayName == "Health checks").Should().Be(2);
        endpoints.Should().Contain(e => e.Metadata.OfType<IAuthorizeData>().Any(a => a.Policy == AuthorizationPolicies.RequireManagerUser));
        endpoints.Should().Contain(e => e.Metadata.OfType<IAuthorizeData>().Any(a => a.Policy == AuthorizationPolicies.RequireDonorUser));
        endpoints.Should().Contain(e => e.Metadata.OfType<IAuthorizeData>().Any(a => a.Policy == AuthorizationPolicies.RequireManagerOrDonorUser));

        var aliveEndpoint = endpoints
            .OfType<RouteEndpoint>()
            .Single(endpoint => endpoint.RoutePattern.RawText == "/alive");
        var context = new DefaultHttpContext
        {
            RequestServices = app.Services
        };
        context.Response.Body = new MemoryStream();

        aliveEndpoint.RequestDelegate.Should().NotBeNull();
        await aliveEndpoint.RequestDelegate!(context);
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }
}
