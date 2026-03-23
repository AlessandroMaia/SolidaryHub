namespace Identity.UnitTests.Helpers;

internal static class IdentityContextFactory
{
    public static IdentityContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<IdentityContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new IdentityContext(options);
    }
}
