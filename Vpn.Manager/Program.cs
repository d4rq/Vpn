using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Vpn.Core.Abstractions;
using Vpn.Infrastructure.Services;
using Vpn.Infrastructure.Utils;
using Vpn.Postgres;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddDbContext<EfContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddScoped<JwtProvider>();
services.AddScoped<UserService>();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtOptions:SecretKey").Value!)),
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["token"];
                return Task.CompletedTask;
            }
        };
    });

services.Configure<HttpClientOptions>(builder.Configuration.GetSection("HttpClientOptions"));
services.AddScoped<IpService>();
services.AddScoped<WgKeyService>();
services.AddHttpClient();

services.AddControllers();
services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();