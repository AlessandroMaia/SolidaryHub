namespace Campaign.API.Application.Queries.DonationIntents.GetDonationIntentById;

public sealed record GetDonationIntentByIdQuery(int DonationIntentId)
    : IQuery<DonationIntentDetailsViewModel?>;
