
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Auth.Login;

public class LoginCommand : IRequest<Result<string>>
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
