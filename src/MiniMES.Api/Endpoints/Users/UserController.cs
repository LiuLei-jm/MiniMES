using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Application.Features.Auth.Login;
using MiniMES.Application.Features.Users.Commands.CreateUser;

namespace MiniMES.Api.Endpoints.Users;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ISender _sender;
    public UserController(ISender sender)
    {
        _sender = sender;
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command)
    {
        var id = await _sender.Send(command);
        return Ok(id);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        return Ok("admin ok");
    }
}