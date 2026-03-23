using Identity.UnitTests.Helpers;

namespace Identity.UnitTests.Infrastructure.Repositories;

public sealed class UserRepositoryTests
{
    [Fact]
    public async Task Repository_ShouldSupportCrudAndQueries()
    {
        await using var context = IdentityContextFactory.Create();
        var repository = new UserRepository(context);

        var donorRole = UserTestFactory.CreateRole(Roles.Donor, id: 10);
        var managerRole = UserTestFactory.CreateRole(Roles.Manager, id: 20);
        context.Roles.AddRange(donorRole, managerRole);

        var activeUser = UserTestFactory.CreateActiveUser(
            email: "active@example.com",
            cpf: "52998224725",
            id: 1);
        UserTestFactory.AssignRole(activeUser, donorRole);
        activeUser.GenerateRefreshToken("active-token");

        var inactiveUser = UserTestFactory.CreateActiveUser(
            email: "inactive@example.com",
            cpf: "11144477735",
            id: 2);
        UserTestFactory.AssignRole(inactiveUser, managerRole);
        inactiveUser.GenerateRefreshToken("revoked-token");
        inactiveUser.RevokeRefreshToken("revoked-token");
        inactiveUser.Deactivate();

        repository.Add(activeUser);
        context.Users.Add(inactiveUser);
        await context.SaveChangesAsync();

        var temporaryUser = UserTestFactory.CreateActiveUser(
            email: "temp@example.com",
            cpf: "12345678909");
        repository.Add(temporaryUser);
        await context.SaveChangesAsync();

        temporaryUser.UpdateName(new PersonName("Novo", "Nome"));
        repository.Update(temporaryUser);
        repository.Delete(temporaryUser);
        await context.SaveChangesAsync();

        repository.UnitOfWork.Should().BeSameAs(context);

        (await repository.GetByIdAsync(1)).Should().NotBeNull();
        (await repository.GetByIdWithRolesAsync(1))!
            .UserRoles.Should().ContainSingle(ur => ur.Role!.Name == Roles.Donor);
        (await repository.GetByIdWithTokensAsync(1))!
            .RefreshTokens.Should().ContainSingle(rt => rt.Token == "active-token");
        (await repository.GetByEmailAsync("ACTIVE@EXAMPLE.COM"))!
            .Id.Should().Be(1);
        (await repository.GetByRefreshTokenAsync("active-token"))!
            .Id.Should().Be(1);
        (await repository.GetByRefreshTokenAsync("revoked-token"))
            .Should().BeNull();
        (await repository.ExistsByEmailAsync("ACTIVE@EXAMPLE.COM"))
            .Should().BeTrue();
        (await repository.GetActiveUsersAsync())
            .Select(user => user.Id)
            .Should().Contain(1)
            .And.NotContain(2);
        (await repository.GetUsersByRoleAsync("donor"))
            .Should().ContainSingle(user => user.Id == 1);
        (await repository.GetRoleByIdAsync(10))!.Name.Should().Be(Roles.Donor);
        (await repository.GetRoleByNameAsync("ngo_manager"))!.Name.Should().Be(Roles.Manager);
        (await repository.GetAllRolesAsync()).Should().HaveCount(2);
        context.Users.Should().NotContain(user => user.Email.Value == "temp@example.com");
    }
}
