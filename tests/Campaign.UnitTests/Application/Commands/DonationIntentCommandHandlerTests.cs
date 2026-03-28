using Campaign.UnitTests.Helpers;

namespace Campaign.UnitTests.Application.Commands;

public sealed class DonationIntentCommandHandlerTests
{
    private readonly IDonationIntentRepository _donationIntentRepository = Substitute.For<IDonationIntentRepository>();
    private readonly ICampaignRepository _campaignRepository = Substitute.For<ICampaignRepository>();
    private readonly ICampaignIntegrationEventService _integrationEventService = Substitute.For<ICampaignIntegrationEventService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public DonationIntentCommandHandlerTests()
    {
        _donationIntentRepository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
    }

    [Fact]
    public async Task CreateDonationIntent_WithValidCampaign_ShouldPersistAndPublishIntegrationEvent()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 10);
        _campaignRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(campaign);
        _donationIntentRepository.Add(Arg.Any<DonationIntent>())
            .Returns(callInfo =>
            {
                var entity = callInfo.Arg<DonationIntent>();
                CampaignTestFactory.SetEntityId(entity, 77);
                return entity;
            });

        var handler = new CreateDonationIntentCommandHandler(
            _donationIntentRepository,
            _campaignRepository,
            _integrationEventService);

        var result = await handler.Handle(
            new CreateDonationIntentCommand(10, 20, 30m, "brl", "api", "corr-1", "msg-1"),
            CancellationToken.None);

        result.Should().Be(77);
        await _integrationEventService.Received(1).AddAndSaveEventAsync(
            Arg.Is<DonationIntentProcessingIntegrationEvent>(e =>
                e.DonationIntentId == 77 &&
                e.WorkerName == "donation-processor"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateDonationIntent_WhenCampaignDoesNotExist_ShouldThrow()
    {
        _campaignRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns((Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign?)null);

        var handler = new CreateDonationIntentCommandHandler(
            _donationIntentRepository,
            _campaignRepository,
            _integrationEventService);

        var act = async () => await handler.Handle(
            new CreateDonationIntentCommand(10, 20, 30m, "BRL", "api", null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<CampaignDomainException>()
            .WithMessage("Campanha não encontrada");
    }

    [Fact]
    public async Task CreateDonationIntent_WhenCampaignIsInactive_ShouldThrow()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 10);
        campaign.Cancel(1);
        _campaignRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(campaign);

        var handler = new CreateDonationIntentCommandHandler(
            _donationIntentRepository,
            _campaignRepository,
            _integrationEventService);

        var act = async () => await handler.Handle(
            new CreateDonationIntentCommand(10, 20, 30m, "BRL", "api", null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<CampaignDomainException>()
            .WithMessage("A campanha não está ativa para receber doações");
    }

    [Fact]
    public async Task CreateDonationIntent_WhenCampaignIsExpired_ShouldThrow()
    {
        var campaign = CampaignTestFactory.CreateCampaign(
            id: 10,
            startDate: DateTime.UtcNow.AddHours(-2),
            endDate: DateTime.UtcNow.AddMinutes(-1));

        _campaignRepository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns(campaign);

        var handler = new CreateDonationIntentCommandHandler(
            _donationIntentRepository,
            _campaignRepository,
            _integrationEventService);

        var act = async () => await handler.Handle(
            new CreateDonationIntentCommand(10, 20, 30m, "BRL", "api", null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<CampaignDomainException>()
            .WithMessage("A campanha está encerrada");
    }

    [Fact]
    public async Task RejectDonationIntent_ShouldRejectAndPersist()
    {
        var donationIntent = CampaignTestFactory.CreateDonationIntent(id: 1);
        _donationIntentRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(donationIntent);
        var handler = new RejectDonationIntentCommandHandler(_donationIntentRepository);

        await handler.Handle(new RejectDonationIntentCommand(1, "dados inválidos"), CancellationToken.None);

        donationIntent.Status.Should().Be(DonationIntentStatus.Rejected);
        _donationIntentRepository.Received(1).Update(donationIntent);
    }

    [Fact]
    public async Task ProcessDonationIntent_ShouldApplyDonationAndPersist()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 10);
        var donationIntent = CampaignTestFactory.CreateDonationIntent(campaignId: 10, donorUserId: 20, amount: 50m, id: 1);

        _donationIntentRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(donationIntent);
        _campaignRepository.GetByIdWithLedgerAsync(10, Arg.Any<CancellationToken>()).Returns(campaign);

        var handler = new ProcessDonationIntentCommandHandler(_donationIntentRepository, _campaignRepository);

        await handler.Handle(new ProcessDonationIntentCommand(1, "worker-1"), CancellationToken.None);

        donationIntent.Status.Should().Be(DonationIntentStatus.Processed);
        campaign.Total.TotalAmountRaised.Should().Be(50m);
        campaign.DonationLedger.Should().ContainSingle();
        _donationIntentRepository.Received(1).Update(donationIntent);
        _campaignRepository.Received(1).Update(campaign);
    }

    [Fact]
    public async Task FailDonationIntentProcessing_ShouldMarkAsFailedAndPersist()
    {
        var donationIntent = CampaignTestFactory.CreateDonationIntent(id: 1);
        _donationIntentRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(donationIntent);

        var handler = new FailDonationIntentProcessingCommandHandler(_donationIntentRepository);

        await handler.Handle(
            new FailDonationIntentProcessingCommand(1, "worker-1", "falhou"),
            CancellationToken.None);

        donationIntent.Status.Should().Be(DonationIntentStatus.Failed);
        donationIntent.ProcessingLogs.Should().ContainSingle();
        donationIntent.ProcessingLogs.Single().WorkerName.Should().Be("worker-1");
        donationIntent.ProcessingLogs.Single().ErrorMessage.Should().Be("falhou");
        donationIntent.DeadLetters.Should().ContainSingle();
        donationIntent.DeadLetters.Single().ErrorMessage.Should().Be("falhou");
        _donationIntentRepository.Received(1).Update(donationIntent);
        await _donationIntentRepository.UnitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FailDonationIntentProcessing_WhenDonationIntentDoesNotExist_ShouldThrow()
    {
        _donationIntentRepository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns((DonationIntent?)null);

        var handler = new FailDonationIntentProcessingCommandHandler(_donationIntentRepository);

        var act = async () => await handler.Handle(
            new FailDonationIntentProcessingCommand(1, "worker-1", "falhou"),
            CancellationToken.None);

        await act.Should().ThrowAsync<CampaignDomainException>()
            .WithMessage("Intenção de doação não encontrada");
    }
}
