namespace Campaign.UnitTests.Helpers;

internal static class CampaignTestFactory
{
    public static Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign CreateCampaign(
        string title = "Campanha Solidaria",
        string description = "Descricao da campanha",
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal financialGoalAmount = 1000m,
        int createdByUserId = 1,
        int? id = null)
    {
        var campaign = Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign.Create(
            title,
            description,
            startDate ?? DateTime.UtcNow,
            endDate ?? DateTime.UtcNow.AddDays(10),
            financialGoalAmount,
            createdByUserId);

        if (id.HasValue)
            SetEntityId(campaign, id.Value);

        return campaign;
    }

    public static DonationIntent CreateDonationIntent(
        int campaignId = 1,
        int donorUserId = 10,
        decimal amount = 50m,
        string currency = "BRL",
        string source = "api",
        string? correlationId = "corr-1",
        string? messageId = "msg-1",
        int? id = null)
    {
        var donationIntent = DonationIntent.Create(
            campaignId,
            donorUserId,
            amount,
            currency,
            source,
            correlationId,
            messageId);

        if (id.HasValue)
            SetEntityId(donationIntent, id.Value);

        return donationIntent;
    }

    public static void SetEntityId(Entity entity, int id)
    {
        var property = typeof(Entity).GetProperty(nameof(Entity.Id));
        property!.SetValue(entity, id);
    }
}
