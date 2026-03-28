namespace Identity.Infrastructure;

/// <remark>
/// dotnet ef migrations add --startup-project ..\Identity.API --context IdentityContext [migration-name]
/// </remark>
public class IdentityContext(DbContextOptions<IdentityContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity_db");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
        modelBuilder.Entity<Role>().HasData(new { Id = 1, Name = SharedKernel.Security.Roles.Manager }, new { Id = 2, Name = SharedKernel.Security.Roles.Donor });
        base.OnModelCreating(modelBuilder);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);

        return result > 0;
    }
}
