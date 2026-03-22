using Identity.API.Apis;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationAuthorizationPolicies()
    .AddOpenApiDocumentation()
    .AddCustomHealthChecks(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseExceptionHandling();
app.ApplyMigrations();
await app.SeedAdminUserAsync();
app.UseOpenApiDocumentation();
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityEndpoints();
app.MapUserEndpoints();

app.Run();
