using MiniMES.Application.Authentication.Models;
using MiniMES.Domain.Entities;

namespace MiniMES.Application.Authentication.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult> IssueTokenPairAsync(User user, CancellationToken cancellationToken = default);
}
