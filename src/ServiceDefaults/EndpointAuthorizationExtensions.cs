using Microsoft.AspNetCore.Builder;
using SharedKernel;

namespace ServiceDefaults;

public static class EndpointAuthorizationExtensions
{
    public static TBuilder RequireManagerAccess<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.RequireAuthorization(AuthorizationPolicies.RequireManagerUser);
        return builder;
    }

    public static TBuilder RequireDonorAccess<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.RequireAuthorization(AuthorizationPolicies.RequireDonorUser);
        return builder;
    }

    public static TBuilder RequireApplicationUserAccess<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.RequireAuthorization(AuthorizationPolicies.RequireManagerOrDonorUser);
        return builder;
    }
}