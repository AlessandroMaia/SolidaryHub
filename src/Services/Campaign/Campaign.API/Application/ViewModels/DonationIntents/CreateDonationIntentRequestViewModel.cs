namespace Campaign.API.Application.ViewModels.DonationIntents;

public sealed record CreateDonationIntentRequestViewModel
{
    public required decimal Amount { get; init; }
    public string Currency { get; init; } = "BRL";
}
