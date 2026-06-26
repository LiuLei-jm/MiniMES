namespace MiniMES.Application.Common.Security;

public interface ICurrentUser
{
    Guid UserId { get; }
    string UserName { get; }
    List<string> Roles { get; }
    bool IsAuthenticated { get; }
}
