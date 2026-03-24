namespace Campaign.API.Application.Commands.DonationIntents.ProcessDonationIntent;

public sealed record ProcessDonationIntentCommand(
    int DonationIntentId,
    string WorkerName,
    Guid RequestId = default) : IIdempotentCommand;
