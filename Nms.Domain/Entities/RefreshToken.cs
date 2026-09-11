using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class RefreshToken : BaseEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private RefreshToken() { }

    public RefreshToken(Guid id, Guid userId, string tokenHash, DateTime expiresAtUtc) : base(id)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
        IsRevoked = false;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAtUtc;
    public void Revoke() => IsRevoked = true;
}