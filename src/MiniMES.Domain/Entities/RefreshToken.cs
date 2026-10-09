using MiniMES.Domain.Common;

namespace MiniMES.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public bool IsRevoked => RevokedAt != null;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsExpired && !IsRevoked;
    public User User { get; set; } = default!;

    public void Revoke(string replacedByTokenHash, string? revokedByIp = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByTokenHash;
        RevokedByIp = revokedByIp;
    }
}
