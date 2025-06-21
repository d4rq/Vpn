namespace Vpn.Postgres.Entities;

public class Config : EntityBase
{
    public string Name { get; set; }
    
    public string PrivateKey { get; set; }
    
    public string PublicKey { get; set; }
    
    public string Ips { get; set; }
    
    public string Dns { get; set; }
    
    public string Endpoint { get; set; }
    
    public string AllowedIps { get; set; }
    
    public string ServerKey { get; set; }
    
    public User? User { get; set; }
    
    public Guid UserId { get; set; }
}