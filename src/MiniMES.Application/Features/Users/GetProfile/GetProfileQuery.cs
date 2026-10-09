using MiniMES.Application.Common.Models;
using MiniMES.Application.Features.Users.Models;

namespace MiniMES.Application.Features.Users.GetProfile;

public sealed record GetProfileQuery : IRequest<Result<UserDetails>>;
