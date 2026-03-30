using Campaign.API.Apis;
using Campaign.API.Extensions;
using Campaign.API.Observability;
using OpenTelemetry.Metrics;
using Serilog;
using ServiceDefaults.Observability;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddObservability()
    .AddPrometheusMetrics()
    .ConfigureEventBus();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics.AddMeter(CampaignMetrics.MeterName));

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationAuthorizationPolicies()
    .AddCustomHealthChecks(builder.Configuration)
    .AddOpenApiDocumentation();

var app = builder.Build();

app.UseRequestCorrelation();
app.UseSerilogRequestLogging();
app.UseExceptionHandling();
app.ApplyMigrations();
app.MapDefaultEndpoints();
app.UsePrometheusMetrics();
app.UseOpenApiDocumentation();
app.UseAuthentication();
app.UseAuthorization();
app.MapCampaignApi();
app.MapDonationIntentApi();

app.Run();
