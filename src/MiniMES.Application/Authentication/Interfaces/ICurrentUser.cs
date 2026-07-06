namespace MiniMES.Application.Authentication.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string UserName { get; }
    List<string> Roles { get; }
    bool IsAuthenticated { get; }
}
