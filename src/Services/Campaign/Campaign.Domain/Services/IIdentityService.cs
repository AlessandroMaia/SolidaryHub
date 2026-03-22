namespace Campaign.Domain.Services;

public interface IIdentityService
{
    string? GetUserIdentity();

    string? GetUserName();

    bool IsInRole(string role);
}
