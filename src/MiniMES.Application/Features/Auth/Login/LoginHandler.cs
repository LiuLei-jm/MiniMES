using MediatR;
using MiniMES.Application.Common.Security;
using MiniMES.Application.Common.Results;
using MiniMES.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Application.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwt;
    public LoginHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwt)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
    }

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUserNameAsync(request.UserName);
        if (user == null) return Result<string>.Failure("User not found");
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<string>.Failure("Invalid password.");
        var roles = new List<string> { "Admin" };
        var token = _jwt.Generate(user, roles);
        return Result<string>.Success(token);
    }
}
