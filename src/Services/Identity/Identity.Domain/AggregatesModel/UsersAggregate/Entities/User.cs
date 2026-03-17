namespace Identity.Domain.AggregatesModel.UsersAggregate.Entities;

public class User : Entity, IAggregateRoot
{
    public PersonName Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Password PasswordHash { get; private set; } = null!;
    public Cpf Cpf { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    protected User() { }

    public static User Create(Email email, Password passwordHash, PersonName name, Cpf cpf)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            Name = name,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            Cpf = cpf
        };

        return user;
    }

    public void ChangePassword(Password newPasswordHash)
    {
        if (Status != UserStatus.Active)
            throw new IdentityDomainException("Não é possível alterar a senha de um usuário desativado");

        PasswordHash = newPasswordHash;
    }

    public void UpdateName(PersonName newName)
        => Name = newName;

    public void Activate()
    {
        if (Status == UserStatus.Active)
            return;

        Status = UserStatus.Active;
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Inactive)
            return;

        Status = UserStatus.Inactive;
        RevokeAllRefreshTokens();
    }

    public void RecordLogin() =>
        LastLoginAt = DateTime.UtcNow;

    public void AssignRole(Role role)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.Id))
            return;

        var userRole = new UserRole(Id, role.Id);
        _userRoles.Add(userRole);
    }

    public void RemoveRole(Role role)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == role.Id);

        if (userRole is not null)
            _userRoles.Remove(userRole);
    }

    public bool HasRole(string roleName)
        => _userRoles.Any(ur => ur.Role?.Name == roleName);

    public RefreshToken GenerateRefreshToken(string token, int expirationDays = 7)
    {
        var refreshToken = new RefreshToken(
            Id,
            token,
            DateTime.UtcNow.AddDays(expirationDays));

        _refreshTokens.Add(refreshToken);
        return refreshToken;
    }

    public void RevokeRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.FirstOrDefault(rt => rt.Token == token);
        refreshToken?.Revoke();
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(rt => rt.IsActive))
            token.Revoke();
    }

    public RefreshToken? GetActiveRefreshToken(string token)
        => _refreshTokens.FirstOrDefault(rt => rt.Token == token && rt.IsActive);

    public bool ValidatePassword(Password inputPasswordHash)
        => PasswordHash.Equals(inputPasswordHash);
}
