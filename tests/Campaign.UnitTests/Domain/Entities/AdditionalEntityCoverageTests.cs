namespace Campaign.UnitTests.Domain.Entities;

public sealed class AdditionalEntityCoverageTests
{
    [Fact]
    public void CampaignDomainException_Constructors_ShouldBeCovered()
    {
        var inner = new InvalidOperationException("inner");

        new CampaignDomainException().Message.Should().NotBeNull();
        var exception = new CampaignDomainException("mensagem", inner);

        exception.Message.Should().Be("mensagem");
        exception.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void ProtectedConstructors_ShouldBeInstantiableForEf()
    {
        Activator.CreateInstance(typeof(CampaignStatusHistory), nonPublic: true).Should().NotBeNull();
        Activator.CreateInstance(typeof(DonationIntentDeadLetter), nonPublic: true).Should().NotBeNull();
        Activator.CreateInstance(typeof(DonationIntentProcessingLog), nonPublic: true).Should().NotBeNull();
        Activator.CreateInstance(typeof(DonationLedgerEntry), nonPublic: true).Should().NotBeNull();
    }
}
