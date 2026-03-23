namespace Campaign.UnitTests.Domain.Entities;

public sealed class DonationIntentProcessingLogTests
{
    [Fact]
    public void Create_WithValidData_ShouldTrimWorkerName()
    {
        var log = DonationIntentProcessingLog.Create(1, 2, " worker ", WorkerProcessingStatus.Processing, null);

        log.DonationIntentId.Should().Be(1);
        log.CampaignId.Should().Be(2);
        log.WorkerName.Should().Be("worker");
        log.Status.Should().Be(WorkerProcessingStatus.Processing);
        log.AttemptCount.Should().Be(1);
        log.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyWorkerName_ShouldThrow()
    {
        var act = () => DonationIntentProcessingLog.Create(1, 2, "", WorkerProcessingStatus.Failed, "erro");

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("O nome do worker é obrigatório.");
    }
}
