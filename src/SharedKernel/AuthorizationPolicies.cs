namespace SharedKernel;

public static class AuthorizationPolicies
{
    public const string RequireManagerUser = nameof(RequireManagerUser);
    public const string RequireDonorUser = nameof(RequireDonorUser);
    public const string RequireManagerOrDonorUser = nameof(RequireManagerOrDonorUser);
}