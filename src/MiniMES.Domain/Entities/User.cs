using MiniMES.Domain.Common;

namespace MiniMES.Domain.Entities;

public class User : BaseEntity
{
    private User() { }

    public User(string userName, string passwordHash)
    {
        Id = Guid.NewGuid();
        UserName = userName;
        PasswordHash = passwordHash;
        IsActive = true;
        CreatedTime = DateTime.UtcNow;
    }

    public string UserName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int FailedLoginCount { get; private set; }
    public DateTime CreatedTime { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = [];
    public void Disable()
    {
        IsActive = false;
    }
    public void Update(string userName, bool isActive)
    {
        UserName = userName;
        IsActive = isActive;
    }
    public void AddFailedLoginAttempt()
    {
        FailedLoginCount++;
    }
    public void ClearFailedLoginAttempts()
    {
        FailedLoginCount = 0;
    }
    public void ModifyLockoutEnd(DateTime? dateTime)
    {
        LockoutEnd = dateTime;
    }
    public void LoginSucceeded()
    {
        FailedLoginCount = 0;
        LockoutEnd = null;
        LastLoginAt = DateTime.UtcNow;
    }
}
