namespace Campaign.API.Application.Queries.DonationIntents.GetPendingDonationIntents;

public sealed record GetPendingDonationIntentsQuery(int Page = 1, int PageSize = 10)
    : IQuery<PagedResponse<DonationIntentListItemViewModel>>;
