using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Vpn.Infrastructure.Services;
using Vpn.Postgres.Entities;
using RegisterRequest = Vpn.Core.Requests.RegisterRequest;

namespace Vpn.Manager.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    [HttpPost("register")]
    public async Task Register(
        [FromServices] UserService userService,
        [FromBody] RegisterRequest request)
        => await userService.Register(request.Username, request.Password);

    [HttpPost("login")]
    public async Task<string> Login(
        [FromServices] UserService userService,
        [FromBody] RegisterRequest request)
    {
        var token = await userService.Login(request.Username, request.Password);
        
        HttpContext.Response.Cookies.Append("token", token);
        return token;
    }
}