using Campaign.API.Application.DomainEventHandlers.Campaigns;
using Campaign.API.Application.DomainEventHandlers.DonationIntents;

namespace Campaign.UnitTests.Application.DomainEventHandlers;

public sealed class DomainEventHandlerTests
{
    [Fact]
    public async Task CampaignHandlers_ShouldHandleNotifications()
    {
        await new CampaignCreatedDomainEventHandler(NullLogger<CampaignCreatedDomainEventHandler>.Instance)
            .Handle(new CampaignCreatedDomainEvent("Campanha", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 100m), CancellationToken.None);
        await new CampaignUpdatedDomainEventHandler(NullLogger<CampaignUpdatedDomainEventHandler>.Instance)
            .Handle(new CampaignUpdatedDomainEvent(1, "Campanha", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 100m), CancellationToken.None);
        await new CampaignCancelledDomainEventHandler(NullLogger<CampaignCancelledDomainEventHandler>.Instance)
            .Handle(new CampaignCancelledDomainEvent(1, "motivo"), CancellationToken.None);
        await new CampaignCompletedDomainEventHandler(NullLogger<CampaignCompletedDomainEventHandler>.Instance)
            .Handle(new CampaignCompletedDomainEvent(1, "motivo"), CancellationToken.None);
    }

    [Fact]
    public async Task DonationIntentHandlers_ShouldHandleNotifications()
    {
        await new DonationIntentCreatedDomainEventHandler(NullLogger<DonationIntentCreatedDomainEventHandler>.Instance)
            .Handle(new DonationIntentCreatedDomainEvent(1, 2, 10m, "corr"), CancellationToken.None);
        await new DonationIntentProcessedDomainEventHandler(NullLogger<DonationIntentProcessedDomainEventHandler>.Instance)
            .Handle(new DonationIntentProcessedDomainEvent(1, 2, 3, 10m, DateTime.UtcNow), CancellationToken.None);
        await new DonationIntentFailedDomainEventHandler(NullLogger<DonationIntentFailedDomainEventHandler>.Instance)
            .Handle(new DonationIntentFailedDomainEvent(1, 2, "erro"), CancellationToken.None);
    }
}
