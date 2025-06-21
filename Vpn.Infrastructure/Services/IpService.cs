using Vpn.Postgres;

namespace Vpn.Infrastructure.Services;

public class IpService(EfContext efContext)
{
    public string GetLastIp()
        => efContext.Configs.OrderBy(c => c.Ips).LastOrDefault()?.Ips
        ?? "10.0.0.1/24";

    public string GetNewIp()
    {
        var lastDigit = int.Parse(GetLastIp().Split('.').Last().Split('/')[0]);
        return $"10.0.0.{lastDigit + 1}/24";
    }
}