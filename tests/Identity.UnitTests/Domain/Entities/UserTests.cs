using Identity.UnitTests.Helpers;

namespace Identity.UnitTests.Domain.Entities;

public sealed class UserTests
{
    [Fact]
    public void Create_ShouldInitializeActiveUser()
    {
        var user = UserTestFactory.CreateActiveUser();

        user.Status.Should().Be(UserStatus.Active);
        user.RefreshTokens.Should().BeEmpty();
        user.UserRoles.Should().BeEmpty();
        user.LastLoginAt.Should().BeNull();
    }

    [Fact]
    public void ChangePassword_WithInactiveUser_ShouldThrow()
    {
        var user = UserTestFactory.CreateActiveUser();
        user.Deactivate();

        var act = () => user.ChangePassword(Password.FromHash("NEW_HASH"));

        act.Should().Throw<IdentityDomainException>()
            .WithMessage("Não é possível alterar a senha de um usuário desativado");
    }

    [Fact]
    public void UpdateName_ShouldReplaceCurrentName()
    {
        var user = UserTestFactory.CreateActiveUser();

        user.UpdateName(new PersonName("Maria", "Oliveira"));

        user.Name.FullName.Should().Be("Maria Oliveira");
    }

    [Fact]
    public void ActivateAndDeactivate_ShouldToggleStatusAndRevokeTokens()
    {
        var user = UserTestFactory.CreateActiveUser(id: 10);
        var token = user.GenerateRefreshToken("refresh-token");

        user.Deactivate();

        user.Status.Should().Be(UserStatus.Inactive);
        token.IsRevoked.Should().BeTrue();

        user.Activate();

        user.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public void AssignRole_ShouldAvoidDuplicates()
    {
        var user = UserTestFactory.CreateActiveUser(id: 1);
        var role = UserTestFactory.CreateRole(id: 2);

        UserTestFactory.AssignRole(user, role);
        UserTestFactory.AssignRole(user, role);

        user.UserRoles.Should().HaveCount(1);
        user.HasRole(role.Name).Should().BeTrue();
    }

    [Fact]
    public void RemoveRole_ShouldRemoveExistingRole()
    {
        var user = UserTestFactory.CreateActiveUser(id: 1);
        var role = UserTestFactory.CreateRole(id: 2);
        UserTestFactory.AssignRole(user, role);

        user.RemoveRole(role);

        user.UserRoles.Should().BeEmpty();
        user.HasRole(role.Name).Should().BeFalse();
    }

    [Fact]
    public void RecordLogin_ShouldSetLastLoginAt()
    {
        var user = UserTestFactory.CreateActiveUser();
        var before = DateTime.UtcNow;

        user.RecordLogin();

        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void GenerateAndManageRefreshTokens_ShouldReturnActiveTokenAndAllowRevocation()
    {
        var user = UserTestFactory.CreateActiveUser(id: 5);

        var activeToken = user.GenerateRefreshToken("active-token");
        var expiredToken = user.GenerateRefreshToken("expired-token", expirationDays: -1);

        activeToken.IsActive.Should().BeTrue();
        expiredToken.IsExpired.Should().BeTrue();
        user.GetActiveRefreshToken("active-token").Should().BeSameAs(activeToken);
        user.GetActiveRefreshToken("expired-token").Should().BeNull();

        user.RevokeRefreshToken("active-token");

        activeToken.IsRevoked.Should().BeTrue();
        user.GetActiveRefreshToken("active-token").Should().BeNull();
    }

    [Fact]
    public void ValidatePassword_ShouldCompareHashes()
    {
        var user = UserTestFactory.CreateActiveUser(passwordHash: "HASH");

        user.ValidatePassword(Password.FromHash("HASH")).Should().BeTrue();
        user.ValidatePassword(Password.FromHash("OTHER")).Should().BeFalse();
    }

    [Fact]
    public void ActivateDeactivateAndRefreshTokenBehaviors_ShouldHandleNoOpBranches()
    {
        var user = UserTestFactory.CreateActiveUser(id: 1);
        var refreshToken = (RefreshToken)typeof(RefreshToken)
            .GetConstructor(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                binder: null,
                [typeof(int), typeof(string), typeof(DateTime)],
                modifiers: null)!
            .Invoke([1, "token", DateTime.UtcNow.AddMinutes(5)]);

        user.Activate();
        user.Deactivate();
        user.Deactivate();
        refreshToken.Revoke();
        var revokedAt = refreshToken.RevokedAt;
        refreshToken.Revoke();

        user.Status.Should().Be(UserStatus.Inactive);
        revokedAt.Should().Be(refreshToken.RevokedAt);
    }
}
