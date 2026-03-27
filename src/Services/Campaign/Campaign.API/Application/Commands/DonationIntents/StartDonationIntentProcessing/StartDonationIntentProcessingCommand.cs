namespace Campaign.API.Application.Commands.DonationIntents.StartDonationIntentProcessing;

public sealed record StartDonationIntentProcessingCommand(
    int DonationIntentId,
    string WorkerName,
    Guid RequestId = default) : IIdempotentCommand;
