using Campaign.UnitTests.Helpers;

namespace Campaign.UnitTests.Domain.Entities;

public sealed class CampaignTests
{
    [Fact]
    public void Create_ShouldInitializeAggregate()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 1);

        campaign.Status.Should().Be(CampaignStatus.Active);
        campaign.FinancialGoalAmount.Should().Be(1000m);
        campaign.Total.TotalAmountRaised.Should().Be(0);
        campaign.DomainEvents.Should().ContainSingle(e => e is CampaignCreatedDomainEvent);
    }

    [Fact]
    public void UpdateDetails_WithActiveCampaign_ShouldUpdateAndRaiseEvent()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 1);

        campaign.UpdateDetails("Novo titulo", "Nova descricao", DateTime.UtcNow, DateTime.UtcNow.AddDays(5), 2500m);

        campaign.Title.Should().Be("Novo titulo");
        campaign.Description.Should().Be("Nova descricao");
        campaign.FinancialGoalAmount.Should().Be(2500m);
        campaign.UpdatedAt.Should().NotBeNull();
        campaign.DomainEvents.Should().Contain(e => e is CampaignUpdatedDomainEvent);
    }

    [Fact]
    public void UpdateDetails_WithClosedCampaign_ShouldThrow()
    {
        var cancelled = CampaignTestFactory.CreateCampaign(id: 1);
        cancelled.Cancel(10);

        var completed = CampaignTestFactory.CreateCampaign(id: 2);
        completed.Complete(10);

        var updateCancelled = () => cancelled.UpdateDetails("T", "D", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 100m);
        var updateCompleted = () => completed.UpdateDetails("T", "D", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 100m);

        updateCancelled.Should().Throw<CampaignDomainException>();
        updateCompleted.Should().Throw<CampaignDomainException>();
    }

    [Fact]
    public void CancelAndComplete_ShouldRespectStateTransitions()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 1);

        campaign.Cancel(99, "cancelada");
        campaign.Cancel(99, "cancelada");

        campaign.Status.Should().Be(CampaignStatus.Cancelled);
        campaign.StatusHistory.Should().ContainSingle();
        campaign.DomainEvents.Should().Contain(e => e is CampaignCancelledDomainEvent);

        var completeCancelled = () => campaign.Complete(99, "ok");

        completeCancelled.Should().Throw<CampaignDomainException>();

        var completed = CampaignTestFactory.CreateCampaign(id: 2);
        completed.Complete(99, "concluida");
        completed.Complete(99, "concluida");

        completed.Status.Should().Be(CampaignStatus.Completed);
        completed.StatusHistory.Should().ContainSingle();
        completed.DomainEvents.Should().Contain(e => e is CampaignCompletedDomainEvent);

        var cancelCompleted = () => completed.Cancel(99, "nao");

        cancelCompleted.Should().Throw<CampaignDomainException>();
    }

    [Fact]
    public void ApplyDonation_ShouldUpdateLedgerAndTotal()
    {
        var campaign = CampaignTestFactory.CreateCampaign(id: 10);

        campaign.ApplyDonation(5, 30, 50m, "worker", "corr-1");

        campaign.DonationLedger.Should().ContainSingle();
        campaign.Total.TotalAmountRaised.Should().Be(50m);
        campaign.Total.TotalDonationsCount.Should().Be(1);
    }

    [Fact]
    public void ApplyDonation_WithInvalidStateOrAmount_ShouldThrow()
    {
        var cancelled = CampaignTestFactory.CreateCampaign(id: 10);
        cancelled.Cancel(1);

        var invalidStatus = () => cancelled.ApplyDonation(5, 30, 50m, "worker");
        var invalidAmount = () => CampaignTestFactory.CreateCampaign(id: 11).ApplyDonation(5, 30, 0m, "worker");
        var expired = () => CampaignTestFactory.CreateCampaign(
            id: 12,
            startDate: DateTime.UtcNow.AddDays(-2),
            endDate: DateTime.UtcNow.AddSeconds(-1))
            .ApplyDonation(5, 30, 10m, "worker");

        invalidStatus.Should().Throw<CampaignDomainException>();
        invalidAmount.Should().Throw<CampaignDomainException>();
        expired.Should().Throw<CampaignDomainException>();
    }

    [Fact]
    public void Create_WithInvalidValues_ShouldThrow()
    {
        var missingTitle = () => Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign.Create("", "Descricao", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 100m, 1);
        var missingDescription = () => Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign.Create("Titulo", "", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 100m, 1);
        var invalidGoal = () => Campaign.Domain.AggregatesModel.CampaignAggregate.Entities.Campaign.Create("Titulo", "Descricao", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 0m, 1);

        missingTitle.Should().Throw<CampaignDomainException>();
        missingDescription.Should().Throw<CampaignDomainException>();
        invalidGoal.Should().Throw<CampaignDomainException>();
    }
}
