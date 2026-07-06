using Microsoft.Extensions.Logging;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Domain.Entities;

namespace MiniMES.Application.Features.Users.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(IPasswordHasher passwordHasher, IApplicationDbContext db, ILogger<CreateUserCommandHandler> logger)
    {
        _passwordHasher = passwordHasher;
        _db = db;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var exist = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);

        if (exist != null) return Result<Guid>.Failure(UserErrors.UserAlreadyExists);
        var hashed = _passwordHasher.Hash(request.Password);
        var user = new User(request.UserName, hashed);

        await _db.Users.AddAsync(user, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Create user {UserName}.", request.UserName);
        return Result<Guid>.Success(user.Id);
    }
}
