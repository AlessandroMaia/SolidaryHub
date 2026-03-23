namespace Campaign.UnitTests.Infrastructure.Services;

public sealed class IdentityServiceTests
{
    [Fact]
    public void Methods_ShouldReadClaimsFromHttpContext()
    {
        var identity = new ClaimsIdentity(
        [
            new Claim("sub", "12"),
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
}
