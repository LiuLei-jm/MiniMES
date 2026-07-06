namespace MiniMES.Application.Common.Interfaces;

public interface ISecurityAuditService
{
    Task LogAsync(string username, string action, bool success);
}
