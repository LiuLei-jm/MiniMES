namespace MiniMES.Application.Features.Users.CreateUser;

public class BusinessValidator : AbstractValidator<Command>
{
    public BusinessValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(3);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
