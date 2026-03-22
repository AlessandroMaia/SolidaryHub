using Campaign.API.Apis;
using Campaign.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureEventBus();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationAuthorizationPolicies()
    .AddCustomHealthChecks(builder.Configuration)
    .AddOpenApiDocumentation();

var app = builder.Build();

app.UseExceptionHandling();
app.ApplyMigrations();
app.MapDefaultEndpoints();
app.UseOpenApiDocumentation();
app.UseAuthentication();
app.UseAuthorization();
app.MapCampaignApi();
app.MapDonationIntentApi();

app.Run();
