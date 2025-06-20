using System.Diagnostics;
using Serilog;
using Vpn.Core.Abstractions;
using Vpn.WgHandler;
using Vpn.WgHandler.Utils;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

services.AddSingleton<IPeerService, PeerService>();

services.AddControllers();
services.AddSwaggerGen();

var app = builder.Build();

WireGuard.EnsureWgInstalled(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();