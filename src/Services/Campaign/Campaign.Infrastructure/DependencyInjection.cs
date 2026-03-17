using Campaign.Domain.Services;
using Campaign.Infrastructure.Services;

namespace Campaign.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        => services.AddDatabase(configuration)
            .AddRepositories()
            .AddServices();

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CampaignContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("CampaignDb"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "campaign_db");
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                }));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICampaignRepository, CampaignRepository>();
        services.AddScoped<IDonationIntentRepository, DonationIntentRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IIdentityService, IdentityService>();

        return services;
    }
}
