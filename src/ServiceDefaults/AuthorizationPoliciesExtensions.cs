using Microsoft.Extensions.DependencyInjection;
using SharedKernel;

namespace ServiceDefaults;

public static class AuthorizationPoliciesExtensions
{
    public static IServiceCollection AddApplicationAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.RequireManagerUser, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Roles.Manager);
            })
            .AddPolicy(AuthorizationPolicies.RequireDonorUser, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Roles.Donor);
            })
            .AddPolicy(AuthorizationPolicies.RequireManagerOrDonorUser, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(Roles.Manager, Roles.Donor);
            });

        return services;
    }
}