namespace Identity.UnitTests.Helpers;

internal static class UserTestFactory
{
    public static User CreateActiveUser(
        string email = "user@example.com",
        string passwordHash = "HASHED",
        string firstName = "Joao",
        string lastName = "Silva",
        string cpf = "52998224725",
        int? id = null)
    {
        var user = User.Create(
            new Email(email),
            Password.FromHash(passwordHash),
            new PersonName(firstName, lastName),
            new Cpf(cpf));

        if (id.HasValue)
            SetEntityId(user, id.Value);

        return user;
    }

    public static Role CreateRole(string name = "DONOR", int? id = null)
    {
        var role = new Role(name);

        if (id.HasValue)
            SetEntityId(role, id.Value);

        return role;
    }

    public static void AssignRole(User user, Role role)
    {
        if (user.IsTransient())
            SetEntityId(user, 1);

        if (role.IsTransient())
            SetEntityId(role, 1);

        user.AssignRole(role);

        var userRole = user.UserRoles.Single(ur => ur.RoleId == role.Id);

        SetProperty(userRole, nameof(UserRole.Role), role);
        SetProperty(userRole, nameof(UserRole.User), user);
    }

    public static void SetEntityId(Entity entity, int id)
    {
        var property = typeof(Entity).GetProperty(nameof(Entity.Id));
        property!.SetValue(entity, id);
    }

    private static void SetProperty(object target, string propertyName, object? value)
    {
        var property = target.GetType().GetProperty(
            propertyName,
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic);

        property!.SetValue(target, value);
    }
}
