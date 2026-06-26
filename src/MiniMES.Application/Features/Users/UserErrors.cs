using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Users;

public static class UserErrors
{
    public static readonly Error UserAlreadyExists =
        new("Users.AlreadyExists", "用户名已经存在");
}
