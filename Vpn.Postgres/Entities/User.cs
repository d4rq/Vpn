namespace Vpn.Postgres.Entities;

public class User : EntityBase
{
    public string UserName { get; set; }
    
    public string PasswordHash { get; set; }
    
    public bool IsAdmin { get; set; }
    
    public List<Config>? Configs { get; set; }
}