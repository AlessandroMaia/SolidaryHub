using System.Diagnostics;

namespace ServiceDefaults.Middleware;

public sealed class RequestCorrelationMiddleware(RequestDelegate next, ILogger<RequestCorrelationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers["x-requestid"].FirstOrDefault() ?? context.TraceIdentifier;
        var correlationId = context.Request.Headers["x-correlation-id"].FirstOrDefault() ?? context.TraceIdentifier;

        using (logger.BeginScope(new Dictionary<string, object?>
        {
            ["request_id"] = requestId,
            ["correlation_id"] = correlationId,
            ["trace_id"] = Activity.Current?.TraceId.ToString()
        }))
        {
            context.Items["request_id"] = requestId;
            context.Items["correlation_id"] = correlationId;

            await next(context);
        }
    }
}

public static class RequestCorrelationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCorrelation(this IApplicationBuilder app)
        => app.UseMiddleware<RequestCorrelationMiddleware>();
}