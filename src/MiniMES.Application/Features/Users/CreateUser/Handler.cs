using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Security;
using MiniMES.Domain.Entities;

namespace MiniMES.Application.Features.Users.CreateUser;

public class Handler : IRequestHandler<Command, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;

    public Handler(IPasswordHasher passwordHasher, IApplicationDbContext db)
    {
        _passwordHasher = passwordHasher;
        _db = db;
    }

    public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
    {
        var exist = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);

        if (exist != null) return Result<Guid>.Failure(UserErrors.UserAlreadyExists);
        var hashed = _passwordHasher.Hash(request.Password);
        var user = new User(request.UserName, hashed);

        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(user.Id);
    }
}
