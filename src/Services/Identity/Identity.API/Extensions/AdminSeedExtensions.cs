namespace Identity.API.Extensions;

public static class AdminSeedExtensions
{
    public static async Task SeedAdminUserAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();
        var userRepository = services.GetRequiredService<IUserRepository>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        var logger = services.GetRequiredService<ILoggerFactory>()
            .CreateLogger("IdentityAdminSeed");

        var settings = configuration.GetSection(AdminSeedSettings.SectionName)
            .Get<AdminSeedSettings>();

        if (settings is null || !settings.Enabled)
        {
            logger.LogInformation("Seed do usuário administrador desabilitado.");
            return;
        }

        if (await userRepository.ExistsByEmailAsync(settings.Email))
        {
            logger.LogInformation("Usuário administrador já cadastrado com o e-mail {Email}.", settings.Email);
            return;
        }

        var managerRole = await userRepository.GetRoleByNameAsync(Roles.Manager);

        if (managerRole is null)
        {
            logger.LogWarning("Perfil de administrador não encontrado. Seed do usuário administrador não executado.");
            return;
        }

        Password.ValidateStrength(settings.Password);

        var adminUser = User.Create(
            new Email(settings.Email),
            Password.FromHash(passwordHasher.Hash(settings.Password)),
            new PersonName(settings.FirstName, settings.LastName),
            new Cpf(settings.Cpf));

        adminUser.AssignRole(managerRole);

        userRepository.Add(adminUser);
        await userRepository.UnitOfWork.SaveEntitiesAsync();

        logger.LogInformation("Usuário administrador criado com o e-mail {Email}.", settings.Email);
    }

    private sealed class AdminSeedSettings
    {
        public const string SectionName = "AdminSeed";

        public bool Enabled { get; init; }
        public string FirstName { get; init; } = "Administrador";
        public string LastName { get; init; } = "ONG";
        public string Email { get; init; } = "admin@conexaosolidaria.local";
        public string Password { get; init; } = "change-me-admin-seed-password";
        public string Cpf { get; init; } = "52998224725";
    }
}
