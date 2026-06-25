namespace MiniMES.Domain.Entities;

public class User
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

    public Guid Id { get; private set; }
    public string UserName { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedTime { get; private set; }

    public void Disable()
    {
        IsActive = false;
    }
}
