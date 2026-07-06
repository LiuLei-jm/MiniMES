namespace MiniMES.Application.Authentication.Interfaces;

public interface ILoginAttemptService
{
    bool IsBlocked(string username);
    void RecordFailure(string username);
    void Reset(string username);
}
