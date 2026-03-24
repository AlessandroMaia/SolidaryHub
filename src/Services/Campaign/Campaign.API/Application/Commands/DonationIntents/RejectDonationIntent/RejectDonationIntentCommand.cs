namespace Campaign.API.Application.Commands.DonationIntents.RejectDonationIntent;

public sealed record RejectDonationIntentCommand(
    int DonationIntentId,
    string Reason,
    Guid RequestId = default) : IIdempotentCommand;
