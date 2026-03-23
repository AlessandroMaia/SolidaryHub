namespace Identity.UnitTests.Domain.Entities;

public sealed class RoleTests
{
    [Fact]
    public void Constructor_WithValidName_ShouldNormalizeToUpper()
    {
        var role = new Role("donor");

        role.Name.Should().Be("DONOR");
        role.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        var act = () => new Role("");

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("O nome do perfil é obrigatório");
    }

    [Fact]
    public void UserRole_Constructors_ShouldInitializeValues()
    {
        var userRole = (UserRole)typeof(UserRole)
            .GetConstructor(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                binder: null,
                [typeof(int), typeof(int)],
                modifiers: null)!
            .Invoke([10, 20]);

        userRole.UserId.Should().Be(10);
        userRole.RoleId.Should().Be(20);
        userRole.AssignedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        Activator.CreateInstance(typeof(UserRole), nonPublic: true).Should().NotBeNull();
    }

    [Fact]
    public void RefreshToken_ProtectedConstructor_ShouldBeInstantiableForEf()
    {
        Activator.CreateInstance(typeof(RefreshToken), nonPublic: true).Should().NotBeNull();
    }
}
