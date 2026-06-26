namespace MiniMES.Api.Endpoints.Users.CreateUser;

public sealed class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(30);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
