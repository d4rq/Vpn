using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Vpn.Manager.Controllers;

[ApiController]
[Authorize]
public class TestController : ControllerBase
{
    [HttpGet("asd")]
    public Task<string> Get() => Task.FromResult("Hello World!");
}