using Campaign.UnitTests.Helpers;

namespace Campaign.UnitTests.Application.Queries;

public sealed class DonationIntentQueryHandlerTests
{
    [Fact]
    public async Task DonationIntentQueries_ShouldFilterAndProjectResults()
    {
        await using var context = CampaignContextFactory.Create();
        var pending = CampaignTestFactory.CreateDonationIntent(campaignId: 1, donorUserId: 10, id: 1);
        var processed = CampaignTestFactory.CreateDonationIntent(campaignId: 1, donorUserId: 20, id: 2);
        processed.MarkAsProcessing("worker");
        processed.MarkAsProcessed("worker");

        context.DonationIntents.AddRange(pending, processed);
        await context.SaveChangesAsync();

        var byIdHandler = new GetDonationIntentByIdQueryHandler(context);
        var byCampaignHandler = new GetDonationIntentsByCampaignQueryHandler(context);
        var byDonorHandler = new GetDonationIntentsByDonorQueryHandler(context);
        var pendingHandler = new GetPendingDonationIntentsQueryHandler(context);

        var byId = await byIdHandler.Handle(new GetDonationIntentByIdQuery(2), CancellationToken.None);
        var byCampaign = await byCampaignHandler.Handle(new GetDonationIntentsByCampaignQuery(1), CancellationToken.None);
        var byDonor = await byDonorHandler.Handle(new GetDonationIntentsByDonorQuery(20), CancellationToken.None);
        var pendings = await pendingHandler.Handle(new GetPendingDonationIntentsQuery(1, 10), CancellationToken.None);

        byId!.Id.Should().Be(2);
        byId.Status.Should().Be(nameof(DonationIntentStatus.Processed));

        byCampaign.TotalRecords.Should().Be(2);
        byCampaign.PageNumber.Should().Be(1);
        byDonor.Data.Should().ContainSingle(d => d.DonorUserId == 20);
        pendings.Data.Should().ContainSingle(d => d.Status == nameof(DonationIntentStatus.Pending));
    }
}
