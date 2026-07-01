namespace MiniMES.Application.Features.Identity.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty()
                                .MaximumLength(50);
        RuleFor(x => x.Password).NotEmpty()
            .MaximumLength(100);
    }
}
