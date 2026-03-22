namespace Campaign.API.Application.Queries.DonationIntents.GetDonationIntentsByCampaign;

public sealed record GetDonationIntentsByCampaignQuery(int CampaignId, int Page = 1, int PageSize = 10)
    : IQuery<PagedResponse<DonationIntentListItemViewModel>>;
