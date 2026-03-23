namespace Campaign.UnitTests.Helpers;

internal static class CampaignContextFactory
{
    public static CampaignContext Create(string? dbName = null, IMediator? mediator = null)
    {
        var options = new DbContextOptionsBuilder<CampaignContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        return new CampaignContext(options, mediator);
    }
}
