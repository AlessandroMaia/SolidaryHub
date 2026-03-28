using Identity.UnitTests.Helpers;

namespace Identity.UnitTests.Application.Queries;

public sealed class UserQueryHandlerTests
{
    [Fact]
    public async Task GetUserById_ShouldReturnProjectedUser()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var context = IdentityContextFactory.Create();
        var role = UserTestFactory.CreateRole(Roles.Manager, 1);
        var user = UserTestFactory.CreateActiveUser(id: 10);
        UserTestFactory.AssignRole(user, role);

        context.Roles.Add(role);
        context.Users.Add(user);
        await context.SaveChangesAsync(ct);

        var handler = new GetUserByIdQueryHandler(context);

        var result = await handler.Handle(new GetUserByIdQuery(10), ct);

        result.Should().NotBeNull();
        result!.Id.Should().Be(10);
        result.Email.Should().Be("user@example.com");
        result.Roles.Should().ContainSingle(Roles.Manager);
    }

    [Fact]
    public async Task GetAllUsers_ShouldFilterByStatusAndRole()
    {
        var ct = TestContext.Current.CancellationToken;
        await using var context = IdentityContextFactory.Create();
        var donorRole = UserTestFactory.CreateRole(Roles.Donor, 1);
        var managerRole = UserTestFactory.CreateRole(Roles.Manager, 2);

        var donor = UserTestFactory.CreateActiveUser(id: 1, email: "donor@example.com");
        var manager = UserTestFactory.CreateActiveUser(id: 2, email: "manager@example.com");
        var inactive = UserTestFactory.CreateActiveUser(id: 3, email: "inactive@example.com");

        UserTestFactory.AssignRole(donor, donorRole);
        UserTestFactory.AssignRole(manager, managerRole);
        UserTestFactory.AssignRole(inactive, donorRole);
        inactive.Deactivate();

        context.Roles.AddRange(donorRole, managerRole);
        context.Users.AddRange(donor, manager, inactive);
        await context.SaveChangesAsync(ct);

        var handler = new GetAllUsersQueryHandler(context);

        var byRole = await handler.Handle(new GetAllUsersQuery(Role: Roles.Donor), ct);
        var byStatus = await handler.Handle(new GetAllUsersQuery(Status: nameof(UserStatus.Inactive)), ct);

        byRole.TotalRecords.Should().Be(2);
        byRole.Data.Select(u => u.Email).Should().BeEquivalentTo(["donor@example.com", "inactive@example.com"]);
        byStatus.TotalRecords.Should().Be(1);
        byStatus.Data.Should().ContainSingle(u => u.Email == "inactive@example.com");
    }
}
