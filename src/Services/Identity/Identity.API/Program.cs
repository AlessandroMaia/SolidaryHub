using Identity.API.Apis;
using Serilog;
using ServiceDefaults.Observability;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddObservability()
    .AddPrometheusMetrics();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationAuthorizationPolicies()
    .AddOpenApiDocumentation()
    .AddCustomHealthChecks(builder.Configuration);

var app = builder.Build();

app.UseRequestCorrelation();
app.UseSerilogRequestLogging();
app.MapDefaultEndpoints();
app.UsePrometheusMetrics();
app.UseExceptionHandling();
app.ApplyMigrations();

await app.SeedAdminUserAsync();

app.UseOpenApiDocumentation();
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityEndpoints();
app.MapUserEndpoints();

app.Run();
