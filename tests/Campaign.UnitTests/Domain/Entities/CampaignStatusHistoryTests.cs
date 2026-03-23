namespace Campaign.UnitTests.Domain.Entities;

public sealed class CampaignStatusHistoryTests
{
    [Fact]
    public void Create_WithValidReason_ShouldTrimAndCreate()
    {
        var history = CampaignStatusHistory.Create(1, CampaignStatus.Active, CampaignStatus.Completed, "  concluida  ", 7);

        history.CampaignId.Should().Be(1);
        history.OldStatus.Should().Be(CampaignStatus.Active);
        history.NewStatus.Should().Be(CampaignStatus.Completed);
        history.Reason.Should().Be("concluida");
        history.ChangedByUserId.Should().Be(7);
    }

    [Fact]
    public void Create_WithEmptyReason_ShouldThrow()
    {
        var act = () => CampaignStatusHistory.Create(1, null, CampaignStatus.Cancelled, "", 2);

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("O motivo da alteração de status é obrigatório.");
    }
}
