using Microsoft.EntityFrameworkCore;
using Vpn.Postgres.Entities;

namespace Vpn.Postgres;

public class EfContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Config> Configs { get; set; }
    
    public EfContext(DbContextOptions<EfContext> options)
        : base(options) { }
}