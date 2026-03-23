namespace Campaign.UnitTests.Domain.Entities;

public sealed class DonationIntentDeadLetterTests
{
    [Fact]
    public void Create_WithValidData_ShouldTrimErrorMessage()
    {
        var deadLetter = DonationIntentDeadLetter.Create(1, "msg-1", 2, " erro ");

        deadLetter.DonationIntentId.Should().Be(1);
        deadLetter.MessageId.Should().Be("msg-1");
        deadLetter.CampaignId.Should().Be(2);
        deadLetter.ErrorMessage.Should().Be("erro");
    }

    [Fact]
    public void Create_WithEmptyErrorMessage_ShouldThrow()
    {
        var act = () => DonationIntentDeadLetter.Create(1, "msg-1", 2, "");

        act.Should().Throw<CampaignDomainException>()
            .WithMessage("A mensagem de erro é obrigatória.");
    }
}
