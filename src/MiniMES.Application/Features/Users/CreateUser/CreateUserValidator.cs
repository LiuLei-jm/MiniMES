using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Users.CreateUser;

public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IApplicationDbContext _db;

    public CreateUserValidator(IApplicationDbContext db)
    {
        _db = db;
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(30)
            .Must(x => !string.Equals(x, "admin", StringComparison.OrdinalIgnoreCase))
            .WithMessage("admin 用户名禁止创建.")
            .MustAsync(IsAvailable)
            .WithMessage("用户名已存在.");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }

    private Task<bool> IsAvailable(string userName, CancellationToken cancellationToken) =>
        _db.Users.AllAsync(u => u.UserName != userName, cancellationToken);
}
