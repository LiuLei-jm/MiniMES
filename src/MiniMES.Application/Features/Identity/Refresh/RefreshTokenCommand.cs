using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Identity.Refresh;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthenticationResult>>;
