namespace Identity.UnitTests.Infrastructure;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_ShouldRegisterExpectedServices()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:IdentityDb"] = "Host=localhost;Database=identity;Username=user;Password=password"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<IdentityContext>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IUserRepository>().Should().BeOfType<UserRepository>();
        scope.ServiceProvider.GetRequiredService<IIdentityService>().Should().BeOfType<IdentityService>();
        scope.ServiceProvider.GetRequiredService<IPasswordHasher>().Should().BeOfType<PasswordHasher>();
        scope.ServiceProvider.GetRequiredService<ITokenProvider>().Should().BeOfType<TokenProvider>();
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().Should().BeOfType<HttpContextAccessor>();
    }
}
