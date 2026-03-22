namespace Campaign.API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using CampaignContext dbContext =
            scope.ServiceProvider.GetRequiredService<CampaignContext>();

        dbContext.Database.Migrate();
    }
}
