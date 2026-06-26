using Mapster;
using MiniMES.Application.Features.Users.CreateUser;

namespace MiniMES.Api.Endpoints.Users.CreateUser;

public static class Mapper
{
    public static Command ToCommand(this Request request)
    {
        return request.Adapt<Command>();
    }
}
