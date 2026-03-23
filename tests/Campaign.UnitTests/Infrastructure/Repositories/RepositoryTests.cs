using Campaign.UnitTests.Helpers;

namespace Campaign.UnitTests.Infrastructure.Repositories;

public sealed class RepositoryTests
{
    [Fact]
    public async Task CampaignRepository_ShouldAddAndQueryCampaigns()
    {
        await using var context = CampaignContextFactory.Create();
        var repository = new CampaignRepository(context);
        var active = CampaignTestFactory.CreateCampaign(id: 1);
        var completed = CampaignTestFactory.CreateCampaign(id: 2);
        completed.Complete(1);

        repository.Add(active);
        repository.Add(completed);
        await context.SaveChangesAsync();

        var activeCampaigns = await repository.GetActiveAsync();
        var byId = await repository.GetByIdAsync(1);
        var withLedger = await repository.GetByIdWithLedgerAsync(1);

        activeCampaigns.Should().ContainSingle(c => c.Id == 1);
        byId.Should().NotBeNull();
        withLedger.Should().NotBeNull();
    }

    [Fact]
    public async Task DonationIntentRepository_ShouldAddAndQueryDonationIntents()
    {
        await using var context = CampaignContextFactory.Create();
        var repository = new DonationIntentRepository(context);
        var pending = CampaignTestFactory.CreateDonationIntent(id: 1, messageId: "msg-1");
        var processed = CampaignTestFactory.CreateDonationIntent(id: 2, messageId: "msg-2");
        processed.MarkAsProcessing("worker");
        processed.MarkAsProcessed("worker");

        repository.Add(pending);
        repository.Add(processed);
        await context.SaveChangesAsync();

        var byId = await repository.GetByIdAsync(1);
        var byMessageId = await repository.GetByMessageIdAsync("msg-2");
        var pendings = await repository.GetPendingAsync();

        byId.Should().NotBeNull();
        byMessageId.Should().NotBeNull();
        pendings.Should().ContainSingle(d => d.Id == 1);
    }
}
