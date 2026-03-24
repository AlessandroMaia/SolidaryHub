using Campaign.Domain.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Campaign.Infrastructure.Services;

public class IdentityService(IHttpContextAccessor context) : IIdentityService
{
    public string? GetUserIdentity()
    {
        var nameIdentifierClaim = context.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

        if (nameIdentifierClaim is not null)
            return nameIdentifierClaim.Value;

        return context.HttpContext?.User.FindFirst("sub")?.Value;
    }

    public string? GetUserName()
        => context.HttpContext?.User.Identity?.Name;

    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        return context.HttpContext?.User?.IsInRole(role) ?? false;
    }
}
