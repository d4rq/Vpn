using Microsoft.AspNetCore.Mvc;

namespace Vpn.WgHandler.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet]
    [Route("/test")]
    public IActionResult Test() => Ok("Hello World!");
}