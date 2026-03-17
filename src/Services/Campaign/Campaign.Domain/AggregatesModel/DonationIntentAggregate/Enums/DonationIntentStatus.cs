namespace Campaign.Domain.AggregatesModel.DonationIntentAggregate.Enums;

public enum DonationIntentStatus
{
    Pending = 1,
    Validated = 2,
    Published = 3,
    Processing = 4,
    Processed = 5,
    Rejected = 6,
    Failed = 7
}
