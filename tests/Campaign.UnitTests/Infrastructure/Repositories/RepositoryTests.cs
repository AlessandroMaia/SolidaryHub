using Campaign.UnitTests.Helpers;

namespace Campaign.UnitTests.Infrastructure.Repositories;

public sealed class RepositoryTests
{
    [Fact]
    public async Task CampaignRepository_ShouldAddAndQueryCampaigns()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var context = CampaignContextFactory.Create();
        var repository = new CampaignRepository(context);
        var active = CampaignTestFactory.CreateCampaign(id: 1);
        var completed = CampaignTestFactory.CreateCampaign(id: 2);
        completed.Complete(1);

        repository.Add(active);
        repository.Add(completed);
        await context.SaveChangesAsync(ct);

        var activeCampaigns = await repository.GetActiveAsync(ct);
        var byId = await repository.GetByIdAsync(1, ct);
        var withLedger = await repository.GetByIdWithLedgerAsync(1, ct);

        activeCampaigns.Should().ContainSingle(c => c.Id == 1);
        byId.Should().NotBeNull();
        withLedger.Should().NotBeNull();
    }

    [Fact]
    public async Task DonationIntentRepository_ShouldAddAndQueryDonationIntents()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var context = CampaignContextFactory.Create();
        var repository = new DonationIntentRepository(context);
        var pending = CampaignTestFactory.CreateDonationIntent(id: 1, messageId: "msg-1");
        var processed = CampaignTestFactory.CreateDonationIntent(id: 2, messageId: "msg-2");
        processed.MarkAsProcessing("worker");
        processed.MarkAsProcessed("worker");

        repository.Add(pending);
        repository.Add(processed);
        await context.SaveChangesAsync(ct);

        var byId = await repository.GetByIdAsync(1, ct);
        var byMessageId = await repository.GetByMessageIdAsync("msg-2", ct);
        var pendings = await repository.GetPendingAsync(ct);

        byId.Should().NotBeNull();
        byMessageId.Should().NotBeNull();
        pendings.Should().ContainSingle(d => d.Id == 1);
    }
}
