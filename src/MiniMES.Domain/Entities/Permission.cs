namespace MiniMES.Domain.Entities;

public class Permission
{
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    private Permission() { }
    public Permission(string code, string description)
    {
        Id = Guid.NewGuid();
        Code = code;
        Description = description;
    }
    public void Update(string code, string description)
    {
        Code = code;
        Description = description;
    }
}
