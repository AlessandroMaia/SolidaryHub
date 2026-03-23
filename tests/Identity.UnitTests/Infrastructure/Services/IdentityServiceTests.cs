namespace Identity.UnitTests.Infrastructure.Services;

public sealed class IdentityServiceTests
{
    [Fact]
    public void Methods_ShouldReadClaimsFromHttpContext()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "10"),
            new Claim(ClaimTypes.Name, "Maria Souza"),
            new Claim(ClaimTypes.Email, "maria@example.com"),
            new Claim(ClaimTypes.Role, Roles.Manager)
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        var service = new IdentityService(httpContextAccessor);

        service.GetUserIdentity().Should().Be("10");
        service.GetUserName().Should().Be("Maria Souza");
        service.GetUserEmail().Should().Be("maria@example.com");
        service.IsAuthenticated().Should().BeTrue();
        service.IsInRole(Roles.Manager).Should().BeTrue();
        service.GetUserRoles().Should().ContainSingle(Roles.Manager);
    }

    [Fact]
    public void GetUserIdentity_ShouldFallbackToSubClaim()
    {
        var identity = new ClaimsIdentity([new Claim("sub", "22")], "Bearer");
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };

        var service = new IdentityService(accessor);

        service.GetUserIdentity().Should().Be("22");
    }

    [Fact]
    public void Constructor_WithNullAccessor_ShouldThrow()
    {
        var act = () => new IdentityService(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IsInRole_WithEmptyRole_ShouldReturnFalse()
    {
        var service = new IdentityService(new HttpContextAccessor());

        service.IsInRole("").Should().BeFalse();
    }
}
