using Microsoft.AspNetCore.Mvc;
using Vpn.Core.Abstractions;
using Vpn.Core.Models;

namespace Vpn.WgHandler.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet]
    [Route("/test")]
    public IActionResult Test([FromServices] IPeerService peerService) => Ok(peerService.ToString());

    [HttpGet("peers")]
    public List<PeerInfo> Get([FromServices] IPeerService peerService) => peerService.GetPeers();
    
    [HttpPost("peer")]
    public void Post(
        [FromServices] IPeerService peerService,
        [FromBody] PeerInfo peerInfo)
        => peerService.AddPeer(peerInfo);
    
    [HttpDelete("peer")]
    public void Delete(
        [FromServices] IPeerService peerService,
        [FromQuery] string publicKey)
        => peerService.RemovePeer(publicKey);
}