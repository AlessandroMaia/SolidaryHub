namespace Identity.API.Extensions;

public static class AuthorizationPoliciesExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("RequireManagerUser", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context => 
                    context.User.IsInRole(Role.Roles.Manager));
            })

            .AddPolicy("RequireAuthenticatedUser", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.IsInRole(Role.Roles.Donor) ||
                    context.User.IsInRole(Role.Roles.Manager));
            });

        return services;
    }
}
