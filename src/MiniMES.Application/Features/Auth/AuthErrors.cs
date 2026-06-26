using MiniMES.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Application.Features.Auth;

public static class AuthErrors
{
    public static readonly Error UserNotFound = new("Auths.UserNotFound", "用户不存在");
    public static readonly Error InvalidPassword = new("Auths.InvalidPassword", "用户名或密码错误");
}
