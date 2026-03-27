namespace Campaign.API.Application.Commands.DonationIntents.CompleteDonationIntentProcessing;

public sealed record CompleteDonationIntentProcessingCommand(
    int DonationIntentId,
    string WorkerName,
    Guid RequestId = default) : IIdempotentCommand;
