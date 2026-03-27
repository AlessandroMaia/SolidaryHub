namespace Campaign.API.Application.Commands.DonationIntents.FailDonationIntentProcessing;

public sealed record FailDonationIntentProcessingCommand(
    int DonationIntentId,
    string WorkerName,
    string Reason,
    Guid RequestId = default) : IIdempotentCommand;
