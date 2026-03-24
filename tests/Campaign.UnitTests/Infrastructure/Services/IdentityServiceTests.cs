namespace Campaign.UnitTests.Infrastructure.Services;

public sealed class IdentityServiceTests
{
    [Fact]
    public void Methods_ShouldPreferNameIdentifierClaim_WhenAvailable()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "12"),
            new Claim("sub", "99"),
            new Claim(ClaimTypes.Name, "Gestor"),
            new Claim(ClaimTypes.Role, Roles.Manager)
        ], "Bearer");

        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        var service = new Campaign.Infrastructure.Services.IdentityService(accessor);

        service.GetUserIdentity().Should().Be("12");
        service.GetUserName().Should().Be("Gestor");
        service.IsInRole(Roles.Manager).Should().BeTrue();
        service.IsInRole("").Should().BeFalse();
    }

    [Fact]
    public void GetUserIdentity_ShouldFallbackToSubClaim_WhenNameIdentifierIsMissing()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim("sub", "12")
        ], "Bearer");

        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        var service = new Campaign.Infrastructure.Services.IdentityService(accessor);

        service.GetUserIdentity().Should().Be("12");
    }
}
