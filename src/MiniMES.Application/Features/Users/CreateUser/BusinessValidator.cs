using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Users.CreateUser;

public class BusinessValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IApplicationDbContext _db;

    public BusinessValidator(IApplicationDbContext db)
    {
        _db = db;
    }

    public BusinessValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(30)
            .Must(x => x != "admin")
            .WithMessage("admin 用户名禁止创建.")
            .MustAsync(async (x, cancellationToken) =>
            {
                return await IsExists(x, cancellationToken);
            })
            .WithMessage("用户名已存在.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }

    private async Task<bool> IsExists(string x, CancellationToken cancellationToken)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserName == x, cancellationToken) is not null;
    }
}
