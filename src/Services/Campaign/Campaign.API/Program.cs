using Campaign.API.Apis;
using Campaign.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddCustomHealthChecks(builder.Configuration);
builder.Services.AddOpenApiDocumentation();

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