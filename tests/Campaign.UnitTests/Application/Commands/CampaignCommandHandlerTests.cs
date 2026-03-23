using Campaign.UnitTests.Helpers;

namespace Campaign.UnitTests.Application.Commands;

public sealed class CampaignCommandHandlerTests
{
    private readonly ICampaignRepository _repository = Substitute.For<ICampaignRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public CampaignCommandHandlerTests()
    {
        _repository.UnitOfWork.Returns(_unitOfWork);
        _unitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>()).Returns(true);
    }

    [Fact]
    public async Task CreateCampaign_ShouldAddAndPersistCampaign()
    {
        var handler = new CreateCampaignCommandHandler(_repository);
        _repository.Add(Arg.Any<Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign>())
            .Returns(callInfo =>
            {
                var entity = callInfo.Arg<Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign>();
                CampaignTestFactory.SetEntityId(entity, 99);
                return entity;
            });

        var result = await handler.Handle(
            new CreateCampaignCommand(1, "Campanha", "Descricao", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 100m),
            CancellationToken.None);

        result.Should().Be(99);
        _repository.Received(1).Add(Arg.Is<Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign>(c => c.Title == "Campanha"));
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCampaign_WhenCampaignDoesNotExist_ShouldThrow()
    {
        var handler = new UpdateCampaignCommandHandler(_repository);
        _repository.GetByIdAsync(10, Arg.Any<CancellationToken>()).Returns((Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign?)null);

        var act = async () => await handler.Handle(
            new UpdateCampaignCommand(10, "Novo", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 50m),
            CancellationToken.None);

        await act.Should().ThrowAsync<CampaignDomainException>()
            .WithMessage("Campanha com ID 10 não encontrada.");
    }

    [Fact]
    public async Task UpdateCampaign_ShouldUpdateEntityAndPersist()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 1);
        var handler = new UpdateCampaignCommandHandler(_repository);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(campaign);

        await handler.Handle(
            new UpdateCampaignCommand(1, "Novo titulo", "Nova descricao", DateTime.UtcNow, DateTime.UtcNow.AddDays(2), 500m),
            CancellationToken.None);

        campaign.Title.Should().Be("Novo titulo");
        _repository.Received(1).Update(campaign);
        await _unitOfWork.Received(1).SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancelCampaign_ShouldCancelAndPersist()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 1);
        var handler = new CancelCampaignCommandHandler(_repository);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(campaign);

        await handler.Handle(new CancelCampaignCommand(1, 7, "motivo"), CancellationToken.None);

        campaign.Status.Should().Be(CampaignStatus.Cancelled);
        _repository.Received(1).Update(campaign);
    }

    [Fact]
    public async Task CompleteCampaign_ShouldCompleteAndPersist()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 1);
        var handler = new CompleteCampaignCommandHandler(_repository);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(campaign);

        await handler.Handle(new CompleteCampaignCommand(1, 7, "motivo"), CancellationToken.None);

        campaign.Status.Should().Be(CampaignStatus.Completed);
        _repository.Received(1).Update(campaign);
    }
}
