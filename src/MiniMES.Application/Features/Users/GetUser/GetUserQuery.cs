using MiniMES.Application.Common.Models;
using MiniMES.Application.Features.Users.Models;

namespace MiniMES.Application.Features.Users.GetUser;

public sealed record GetUserQuery(Guid UserId) : IRequest<Result<UserDetails>>;
