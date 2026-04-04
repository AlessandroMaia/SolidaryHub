using System.Security.Cryptography;
using System.Text;

namespace Identity.Domain.AggregatesModel.UsersAggregate.Entities;

public class RefreshToken : Entity
{
    public int UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    protected RefreshToken() { }

    internal RefreshToken(int userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = HashToken(token);
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public bool Matches(string token)
        => Token == token || Token == HashToken(token);

    public void Revoke()
    {
        if (!IsRevoked)
            RevokedAt = DateTime.UtcNow;
    }

    public static string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }
}
