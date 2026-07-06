namespace MiniMES.Application.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class PermissionAttribute : Attribute
{
    public string Code { get; }
    public PermissionAttribute(string code)
    {
        Code = code;
    }
}
