namespace Identity.Domain.AggregatesModel.UsersAggregate.Entities;

public class Role : Entity
{
    public string Name { get; private set; } = null!;

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    public Role(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new IdentityDomainException("O nome do perfil é obrigatório");

        Name = name.ToUpperInvariant();
    }

    public static class Roles
    {
        public const string Manager = "NGO_MANAGER";
        public const string Donor = "DONOR";
    }
}
