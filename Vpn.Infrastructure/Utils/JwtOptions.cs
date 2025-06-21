namespace Vpn.Infrastructure.Utils;

public class JwtOptions
{
    public string SecretKey { get; set; }
    
    public int ExpiresHours { get; set; }
}