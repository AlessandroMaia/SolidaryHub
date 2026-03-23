namespace Campaign.UnitTests.Application.Validations;

public sealed class CommandValidatorTests
{
    [Fact]
    public void CampaignValidators_WithInvalidData_ShouldReturnErrors()
    {
        var createValidator = new CreateCampaignCommandValidator();
        var updateValidator = new UpdateCampaignCommandValidator();
        var cancelValidator = new CancelCampaignCommandValidator();
        var completeValidator = new CompleteCampaignCommandValidator();

        createValidator.Validate(new CreateCampaignCommand(0, "", "", DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), 0m)).IsValid.Should().BeFalse();
        updateValidator.Validate(new UpdateCampaignCommand(0, "", "", DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), 0m)).IsValid.Should().BeFalse();
        cancelValidator.Validate(new CancelCampaignCommand(0, 0, null)).IsValid.Should().BeFalse();
        completeValidator.Validate(new CompleteCampaignCommand(0, 0, null)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void DonationIntentValidators_WithInvalidData_ShouldReturnErrors()
    {
        var createValidator = new CreateDonationIntentCommandValidator();
        var processValidator = new ProcessDonationIntentCommandValidator();
        var rejectValidator = new RejectDonationIntentCommandValidator();
        var campaignByIdValidator = new GetCampaignByIdQueryValidator();
        var donationByIdValidator = new GetDonationIntentByIdQueryValidator();
        var campaignDonationsValidator = new GetDonationIntentsByCampaignQueryValidator();
        var donorDonationsValidator = new GetDonationIntentsByDonorQueryValidator();

        createValidator.Validate(new CreateDonationIntentCommand(0, 0, 0m, "", "", null, null)).IsValid.Should().BeFalse();
        processValidator.Validate(new ProcessDonationIntentCommand(0, "")).IsValid.Should().BeFalse();
        rejectValidator.Validate(new RejectDonationIntentCommand(0, "")).IsValid.Should().BeFalse();
        campaignByIdValidator.Validate(new GetCampaignByIdQuery(0)).IsValid.Should().BeFalse();
        donationByIdValidator.Validate(new GetDonationIntentByIdQuery(0)).IsValid.Should().BeFalse();
        campaignDonationsValidator.Validate(new GetDonationIntentsByCampaignQuery(0)).IsValid.Should().BeFalse();
        donorDonationsValidator.Validate(new GetDonationIntentsByDonorQuery(0)).IsValid.Should().BeFalse();
    }
}
