using Microsoft.EntityFrameworkCore;
using Vpn.Core.Abstractions;
using Vpn.Infrastructure.Utils;
using Vpn.Postgres;
using Vpn.Postgres.Entities;

namespace Vpn.Infrastructure.Services;

public class UserService(IPasswordHasher passwordHasher, EfContext dbContext, JwtProvider jwtProvider)
{
    public async Task Register(string username, string password)
    {
        var hash = passwordHasher.Generate(password);
        
        var user = new User { UserName = username, PasswordHash = hash, IsAdmin = false};
        
        await dbContext.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task<string> Login(string username, string password)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.UserName == username);
        
        if (user == null)
            throw new ApplicationException("User not found");
        
        var res = passwordHasher.Verify(password, user.PasswordHash);
        
        if (!res)
            throw new ApplicationException("Invalid password");

        var token = jwtProvider.GenerateToken(user);
        return token;
    }
}