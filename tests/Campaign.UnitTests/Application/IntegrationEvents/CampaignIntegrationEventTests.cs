namespace Campaign.UnitTests.Application.IntegrationEvents;

public sealed class CampaignIntegrationEventTests
{
    [Fact]
    public async Task CampaignIntegrationEventService_ShouldPublishAndClearPendingEvents()
    {
        var ct = TestContext.Current.CancellationToken;
        var eventBus = Substitute.For<IEventBus>();
        var service = new CampaignIntegrationEventService(
            NullLogger<CampaignIntegrationEventService>.Instance,
            eventBus);

        var first = new DonationIntentProcessingIntegrationEvent(1, "donation-processor");
        var second = new DonationIntentProcessingIntegrationEvent(2, "donation-processor");

        await service.AddAndSaveEventAsync(first, ct);
        await service.AddAndSaveEventAsync(second, ct);

        await service.PublishThroughEventBusAsync(Guid.NewGuid(), ct);
        await service.PublishAsync(Guid.NewGuid(), ct);

        await eventBus.Received(1).PublishAsync(first);
        await eventBus.Received(1).PublishAsync(second);
    }

    [Fact]
    public async Task DonationIntentProcessedIntegrationEventHandler_ShouldSendProcessCommand()
    {
        var mediator = Substitute.For<IMediator>();
        var handler = new DonationIntentProcessedIntegrationEventHandler(
            NullLogger<DonationIntentProcessedIntegrationEventHandler>.Instance,
            mediator);

        await handler.Handle(new DonationIntentProcessedIntegrationEvent(7, "worker-1"));

        var sentCommand = mediator.ReceivedCalls()
            .Select(call => call.GetArguments().FirstOrDefault())
            .OfType<ProcessDonationIntentCommand>()
            .Single();

        sentCommand.DonationIntentId.Should().Be(7);
        sentCommand.WorkerName.Should().Be("worker-1");
    }

    [Fact]
    public async Task DonationIntentFailedIntegrationEventHandler_ShouldSendFailCommand()
    {
        var mediator = Substitute.For<IMediator>();
        var handler = new DonationIntentFailedIntegrationEventHandler(
            NullLogger<DonationIntentFailedIntegrationEventHandler>.Instance,
            mediator);

        await handler.Handle(new DonationIntentFailedIntegrationEvent(8, "worker-2", "falhou"));

        var sentCommand = mediator.ReceivedCalls()
            .Select(call => call.GetArguments().FirstOrDefault())
            .OfType<FailDonationIntentProcessingCommand>()
            .Single();

        sentCommand.DonationIntentId.Should().Be(8);
        sentCommand.WorkerName.Should().Be("worker-2");
        sentCommand.Reason.Should().Be("falhou");
    }
}
