namespace Campaign.API.Application.Queries.DonationIntents.GetDonationIntentsByDonor;

public sealed record GetDonationIntentsByDonorQuery(int DonorUserId, int Page = 1, int PageSize = 10)
    : IQuery<PagedResponse<DonationIntentListItemViewModel>>;
