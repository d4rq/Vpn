using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Vpn.Core.Models;
using Vpn.Core.Requests;
using Vpn.Infrastructure.Services;
using Vpn.Infrastructure.Utils;
using Vpn.Postgres;
using Vpn.Postgres.Entities;

namespace Vpn.Manager.Controllers;

[Authorize]
[ApiController]
public class ConfigController : ControllerBase
{
    [HttpPost("config")]
    public async Task<string> CreateConfig(
        IOptions<HttpClientOptions> options,
        [FromServices] IpService ipService,
        [FromServices] WgKeyService keyService,
        [FromServices] EfContext efContext,
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromBody] CreateConfigRequest request)
    {
        var handler = new JwtSecurityTokenHandler();
        var userId = handler.ReadJwtToken(HttpContext.Request.Cookies["token"]).Claims.First(c => c.Type == "userId").Value;
        var user = efContext.Users
            .Include(u => u.Configs)
            .First(u => u.Id.ToString() == userId);
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(options.Value.WgHandlerAddress);
        
        var privateKey = keyService.GeneratePrivateKey();
        var config = new Config
        {
            Name = request.Name,
            Dns = request.Dns,
            Ips = ipService.GetNewIp(),
            AllowedIps = "0.0.0.0/0",
            Endpoint = "217.66.21.162:51820",
            PrivateKey = privateKey,
            PublicKey = keyService.GeneratePublicKey(privateKey),
            ServerKey = await (await client.GetAsync("publicKey")).Content.ReadAsStringAsync(),
        };

        if (user.Configs == null)
            user.Configs = new List<Config> { config };
        else 
            user.Configs.Add(config);
        await efContext.SaveChangesAsync();
        
        await client.PostAsync("peer", JsonContent.Create(new PeerInfo { PublicKey = config.PublicKey, AllowedIps = config.Ips }));
        
        return config.Name;
    }

    [HttpGet("config")]
    public async Task<string> GetConfig(
        [FromServices] EfContext efContext,
        string configName)
    {
        var handler = new JwtSecurityTokenHandler();
        var userId = handler.ReadJwtToken(HttpContext.Request.Cookies["token"]).Claims.First(c => c.Type == "userId").Value;
        var user = efContext.Users
            .Include(u => u.Configs)
            .First(u => u.Id.ToString() == userId);
        
        var config = user.Configs?.FirstOrDefault(c => c.Name == configName)
            ?? throw new Exception($"Config {configName} not found");

        var res = new List<string>
        {
            "[Interface]",
            $"DNS = {config.Dns}",
            $"PrivateKey = {config.PrivateKey}",
            $"Address = {config.Ips}",
            "",
            "[Peer]",
            $"Endpoint = {config.Endpoint}",
            $"AllowedIps = {config.AllowedIps}",
            $"PublicKey = {config.ServerKey}",
        };
        
        return string.Join("\n", res);
    }
}