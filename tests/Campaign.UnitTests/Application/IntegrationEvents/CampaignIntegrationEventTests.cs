namespace Campaign.UnitTests.Application.IntegrationEvents;

public sealed class CampaignIntegrationEventTests
{
    [Fact]
    public async Task CampaignIntegrationEventService_ShouldPublishAndClearPendingEvents()
    {
        var eventBus = Substitute.For<IEventBus>();
        var service = new CampaignIntegrationEventService(
            NullLogger<CampaignIntegrationEventService>.Instance,
            eventBus);

        var first = new DonationIntentReceivedIntegrationEvent(1, 10, 20, 30m, "corr");
        var second = new DonationIntentReceivedIntegrationEvent(2, 11, 21, 40m, "corr-2");

        await service.AddAndSaveEventAsync(first);
        await service.AddAndSaveEventAsync(second);

        await service.PublishThroughEventBusAsync(Guid.NewGuid());
        await service.PublishAsync(Guid.NewGuid());

        await eventBus.Received(1).PublishAsync(first);
        await eventBus.Received(1).PublishAsync(second);
    }

    [Fact]
    public async Task DonationIntentReceivedIntegrationEventHandler_ShouldSendProcessCommand()
    {
        var mediator = Substitute.For<IMediator>();
        var handler = new DonationIntentReceivedIntegrationEventHandler(
            NullLogger<DonationIntentReceivedIntegrationEventHandler>.Instance,
            mediator);

        await handler.Handle(new DonationIntentReceivedIntegrationEvent(7, 1, 2, 10m, "corr"));

        await mediator.Received(1).Send(
            Arg.Is<ProcessDonationIntentCommand>(c => c.DonationIntentId == 7 && c.WorkerName == "rabbitmq-worker"));
    }
}
