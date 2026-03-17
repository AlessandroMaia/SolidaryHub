namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        => services.AddAuthentication(configuration)
            .AddDatabase(configuration)
            .AddRepositories()
            .AddServices();

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("IdentityDb"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identity_db");
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                }));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();

        return services;
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenSettings>(
            configuration.GetSection(TokenSettings.SectionName));

        var tokenSettings = configuration
            .GetSection(TokenSettings.SectionName)
            .Get<TokenSettings>()!
                ?? throw new InvalidOperationException($"A seção de configuração '{TokenSettings.SectionName}' não foi encontrada");

        if (string.IsNullOrWhiteSpace(tokenSettings.Secret) || tokenSettings.Secret.Length < 32)
            throw new InvalidOperationException("A chave secreta do token deve conter no mínimo 32 caracteres");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret)),

                ValidateIssuer = true,
                ValidIssuer = tokenSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = tokenSettings.Audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                        context.Response.Headers.Add(new KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>("token-expired", "true"));

                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}
