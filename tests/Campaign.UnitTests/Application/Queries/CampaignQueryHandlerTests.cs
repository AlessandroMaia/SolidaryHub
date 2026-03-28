using Campaign.UnitTests.Helpers;

namespace Campaign.UnitTests.Application.Queries;

public sealed class CampaignQueryHandlerTests
{
    [Fact]
    public async Task GetCampaignById_ShouldReturnDetails()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var context = CampaignContextFactory.Create();
        var campaign = CampaignTestFactory.CreateCampaign(id: 1);
        context.Campaigns.Add(campaign);
        await context.SaveChangesAsync(ct);

        var handler = new GetCampaignByIdQueryHandler(context);
        var result = await handler.Handle(new GetCampaignByIdQuery(1), ct);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Title.Should().Be(campaign.Title);
    }

    [Fact]
    public async Task GetActiveCampaigns_ShouldReturnPagedActiveCampaignsOrderedByEndDate()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var context = CampaignContextFactory.Create();
        var first = CampaignTestFactory.CreateCampaign(id: 1, endDate: DateTime.UtcNow.AddDays(1));
        var second = CampaignTestFactory.CreateCampaign(id: 2, endDate: DateTime.UtcNow.AddDays(5));
        var cancelled = CampaignTestFactory.CreateCampaign(id: 3, endDate: DateTime.UtcNow.AddDays(10));
        cancelled.Cancel(1);

        context.Campaigns.AddRange(first, second, cancelled);
        await context.SaveChangesAsync(ct);

        var handler = new GetActiveCampaignsQueryHandler(context);
        var result = await handler.Handle(new GetActiveCampaignsQuery(1, 10), ct);

        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalRecords.Should().Be(2);
        result.Data.Select(c => c.Id).Should().ContainInOrder(2, 1);
    }

    [Fact]
    public async Task GetCampaignsAndPublicPanel_ShouldReturnCorrectPaging()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var context = CampaignContextFactory.Create();
        context.Campaigns.AddRange(
            CampaignTestFactory.CreateCampaign(id: 1),
            CampaignTestFactory.CreateCampaign(id: 2),
            CampaignTestFactory.CreateCampaign(id: 3));
        await context.SaveChangesAsync(ct);

        var campaignsHandler = new GetCampaignsQueryHandler(context);
        var publicPanelHandler = new GetCampaignPublicPanelQueryHandler(context);

        var campaigns = await campaignsHandler.Handle(new GetCampaignsQuery(1, 2), ct);
        var publicPanel = await publicPanelHandler.Handle(new GetCampaignPublicPanelQuery(2, 1), ct);

        campaigns.PageNumber.Should().Be(1);
        campaigns.PageSize.Should().Be(2);
        campaigns.TotalRecords.Should().Be(3);
        campaigns.Data.Should().HaveCount(2);

        publicPanel.PageNumber.Should().Be(2);
        publicPanel.PageSize.Should().Be(1);
        publicPanel.TotalRecords.Should().Be(3);
        publicPanel.Data.Should().HaveCount(1);
    }
}
