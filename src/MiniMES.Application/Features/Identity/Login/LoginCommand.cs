using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Identity.Login;

public sealed record LoginCommand(string UserName, string Password) : IRequest<Result<AuthenticationResult>>;
