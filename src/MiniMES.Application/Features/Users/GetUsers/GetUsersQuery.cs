using MiniMES.Application.Common.Models;
using MiniMES.Application.Features.Users.Models;

namespace MiniMES.Application.Features.Users.GetUsers;

public sealed record GetUsersQuery(string? Keyword, int Page, int PageSize) : IRequest<Result<UserPage>>;
