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
}
