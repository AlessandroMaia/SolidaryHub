namespace ApiGateway.Endpoints;

public static class GatewayEndpoints
{
    public static IEndpointRouteBuilder MapGatewayEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks("/health")
            .AllowAnonymous()
            .ExcludeFromDescription();

        endpoints.MapHealthChecks("/alive")
            .AllowAnonymous()
            .ExcludeFromDescription();

        endpoints.MapGet("/", () => Results.Ok(new
            {
                service = "ApiGateway",
                status = "ok"
            }))
            .AllowAnonymous()
            .ExcludeFromDescription();

        return endpoints;
    }
}
