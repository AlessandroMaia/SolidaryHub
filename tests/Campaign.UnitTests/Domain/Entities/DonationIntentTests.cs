using Campaign.UnitTests.Helpers;

namespace Campaign.UnitTests.Domain.Entities;

public sealed class DonationIntentTests
{
    [Fact]
    public void Create_ShouldInitializePendingIntent()
    {
        var donationIntent = CampaignTestFactory.CreateDonationIntent(id: 1);

        donationIntent.Status.Should().Be(DonationIntentStatus.Pending);
        donationIntent.Currency.Should().Be("BRL");
        donationIntent.DomainEvents.Should().ContainSingle(e => e is DonationIntentCreatedDomainEvent);
    }

    [Fact]
    public void Create_WithInvalidValues_ShouldThrow()
    {
        var invalidCampaign = () => DonationIntent.Create(0, 1, 10m, "BRL", "api");
        var invalidDonor = () => DonationIntent.Create(1, 0, 10m, "BRL", "api");
        var invalidAmount = () => DonationIntent.Create(1, 1, 0m, "BRL", "api");
        var invalidCurrency = () => DonationIntent.Create(1, 1, 10m, "", "api");
        var invalidSource = () => DonationIntent.Create(1, 1, 10m, "BRL", "");

        invalidCampaign.Should().Throw<CampaignDomainException>();
        invalidDonor.Should().Throw<CampaignDomainException>();
        invalidAmount.Should().Throw<CampaignDomainException>();
        invalidCurrency.Should().Throw<CampaignDomainException>();
        invalidSource.Should().Throw<CampaignDomainException>();
    }

    [Fact]
    public void ValidateAndPublish_ShouldMoveThroughExpectedStates()
    {
        var donationIntent = CampaignTestFactory.CreateDonationIntent(id: 1);

        donationIntent.ValidateForProcessing();
        donationIntent.MarkAsPublished();

        donationIntent.Status.Should().Be(DonationIntentStatus.Published);
        donationIntent.ValidatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ValidateAndPublish_WithInvalidCurrentState_ShouldThrow()
    {
        var published = CampaignTestFactory.CreateDonationIntent(id: 1);
        published.ValidateForProcessing();
        published.MarkAsPublished();

        var validateAgain = () => published.ValidateForProcessing();
        var publishWithoutValidation = () => CampaignTestFactory.CreateDonationIntent(id: 2).MarkAsPublished();

        validateAgain.Should().Throw<CampaignDomainException>();
        publishWithoutValidation.Should().Throw<CampaignDomainException>();
    }

    [Fact]
    public void MarkAsProcessing_ShouldAllowPendingAndFailedStates()
    {
        var pending = CampaignTestFactory.CreateDonationIntent(id: 1);
        pending.MarkAsProcessing("worker-1");

        pending.Status.Should().Be(DonationIntentStatus.Processing);
        pending.ProcessingLogs.Should().ContainSingle();

        var failed = CampaignTestFactory.CreateDonationIntent(id: 2);
        failed.MarkAsFailed("worker-1", "erro", sendToDeadLetter: true);
        failed.MarkAsProcessing("worker-2");

        failed.Status.Should().Be(DonationIntentStatus.Processing);
        failed.DeadLetters.Should().ContainSingle();
    }

    [Fact]
    public void MarkAsProcessing_WithUnsupportedState_ShouldThrow()
    {
        var donationIntent = CampaignTestFactory.CreateDonationIntent(id: 1);
        donationIntent.ValidateForProcessing();

        var act = () => donationIntent.MarkAsProcessing("worker");

        act.Should().Throw<CampaignDomainException>();
    }

    [Fact]
    public void MarkAsProcessed_ShouldRequireProcessingState()
    {
        var invalid = () => CampaignTestFactory.CreateDonationIntent(id: 1).MarkAsProcessed("worker");

        invalid.Should().Throw<CampaignDomainException>();

        var donationIntent = CampaignTestFactory.CreateDonationIntent(id: 2);
        donationIntent.MarkAsProcessing("worker");
        donationIntent.MarkAsProcessed("worker");

        donationIntent.Status.Should().Be(DonationIntentStatus.Processed);
        donationIntent.ProcessedAt.Should().NotBeNull();
        donationIntent.DomainEvents.Should().Contain(e => e is DonationIntentProcessedDomainEvent);
    }

    [Fact]
    public void Reject_ShouldValidateReasonAndProcessedState()
    {
        var missingReason = () => CampaignTestFactory.CreateDonationIntent(id: 1).Reject("");

        missingReason.Should().Throw<CampaignDomainException>();

        var processed = CampaignTestFactory.CreateDonationIntent(id: 2);
        processed.MarkAsProcessing("worker");
        processed.MarkAsProcessed("worker");

        var rejectProcessed = () => processed.Reject("motivo");

        rejectProcessed.Should().Throw<CampaignDomainException>();

        var rejected = CampaignTestFactory.CreateDonationIntent(id: 3);
        rejected.Reject("dados inválidos");

        rejected.Status.Should().Be(DonationIntentStatus.Rejected);
        rejected.RejectionReason.Should().Be("dados inválidos");
    }

    [Fact]
    public void MarkAsFailed_ShouldRequireErrorAndOptionallyCreateDeadLetter()
    {
        var missingError = () => CampaignTestFactory.CreateDonationIntent(id: 1).MarkAsFailed("worker", "");

        missingError.Should().Throw<CampaignDomainException>();

        var donationIntent = CampaignTestFactory.CreateDonationIntent(id: 2, messageId: "msg-2");
        donationIntent.MarkAsFailed("worker", "erro de fila", sendToDeadLetter: true);

        donationIntent.Status.Should().Be(DonationIntentStatus.Failed);
        donationIntent.ProcessingLogs.Should().ContainSingle();
        donationIntent.DeadLetters.Should().ContainSingle();
        donationIntent.DomainEvents.Should().Contain(e => e is DonationIntentFailedDomainEvent);
    }
}
