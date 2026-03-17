using Identity.API.Apis;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddOpenApiDocumentation()
    .AddCustomHealthChecks(builder.Configuration);

builder.Services.AddAuthorizationPolicies();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseExceptionHandling();
app.ApplyMigrations();
await app.SeedAdminUserAsync();
app.UseOpenApiDocumentation();
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityEndpoints();

app.Run();
