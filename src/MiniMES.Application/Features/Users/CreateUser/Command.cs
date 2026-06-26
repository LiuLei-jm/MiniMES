using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Users.CreateUser;

public sealed record Command(string UserName, string Password) : IRequest<Result<Guid>>;
