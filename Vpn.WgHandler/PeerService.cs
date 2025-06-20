using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;
using Vpn.Core.Abstractions;
using Vpn.Core.Models;

namespace Vpn.WgHandler;

public class PeerService : IPeerService
{
    private readonly string _configPath;
    private readonly string _publicKey;
    
    public PeerService()
    {
        _configPath = OperatingSystem.IsWindows()
            ? "C:\\Program Files\\WireGuard\\wg0.conf"
            : "/etc/wireguard/wg0.conf";

        if (!File.Exists(_configPath))
        {
            File.Create(_configPath).Close();

            var privateKey = GeneratePrivateKey();
            var defaultConfig = new List<string>()
            {
                "[Interface]",
                "Address = 10.0.0.1/24",
                "ListenPort = 51820",
                $"PrivateKey = {privateKey}",
                "PostUp = iptables -A FORWARD -i wg0 -j ACCEPT; iptables -t nat -A POSTROUTING -o enp6s0 -j MASQUERADE; ip6tables -A FORWARD -i wg0 -j ACCEPT; ip6tables -t nat -A POSTROUTING -o enp6s0 -j MASQUERADE\nPostDown = iptables -D FORWARD -i wg0 -j ACCEPT; iptables -t nat -D POSTROUTING -o enp6s0 -j MASQUERADE; ip6tables -D FORWARD -i wg0 -j ACCEPT; ip6tables -t nat -D POSTROUTING -o enp6s0 -j MASQUERADE",
                "PostUp = iptables -A FORWARD -i wg0 -j ACCEPT; iptables -t nat -A POSTROUTING -o enp6s0 -j MASQUERADE; ip6tables -A FORWARD -i wg0 -j ACCEPT; ip6tables -t nat -A POSTROUTING -o enp6s0 -j MASQUERADE\nPostDown = iptables -D FORWARD -i wg0 -j ACCEPT; iptables -t nat -D POSTROUTING -o enp6s0 -j MASQUERADE; ip6tables -D FORWARD -i wg0 -j ACCEPT; ip6tables -t nat -D POSTROUTING -o enp6s0 -j MASQUERADE"
            };
            
            File.WriteAllLines(_configPath, defaultConfig);

            _publicKey = GeneratePublicKey(privateKey);
            Log.Information("");
        }
        else _publicKey = GeneratePublicKey(
            File.ReadAllLines(_configPath).First(l => l.Contains("PrivateKey")).Split(" = ")[1]);
        Log.Information("Public key is {PublicKey}", _publicKey);
    }
    
    public List<PeerInfo> GetPeers()
    {
        var lines = File.ReadAllLines(_configPath);
        var peers = new List<PeerInfo>();

        PeerInfo currentPeer = null!;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
        
            if (trimmedLine.StartsWith("[Peer]"))
            {
                currentPeer = new PeerInfo();
                peers.Add(currentPeer);
                continue;
            }
        
            if (currentPeer == null) continue;
        
            if (trimmedLine.StartsWith("PublicKey = "))
                currentPeer.PublicKey = trimmedLine["PublicKey = ".Length..].Trim();
        
            else if (trimmedLine.StartsWith("AllowedIPs = "))
                currentPeer.AllowedIps = trimmedLine["AllowedIPs = ".Length..].Trim();
        }
        return peers;
    }

    public void AddPeer(PeerInfo peer)
    {
        ArgumentException.ThrowIfNullOrEmpty(peer.PublicKey);
        ArgumentException.ThrowIfNullOrEmpty(peer.AllowedIps);
        
        var existingPeers = GetPeers();
        
        if (existingPeers.Any(p => p.PublicKey == peer.PublicKey))
            throw new InvalidOperationException("Public key already exists");

        var peerConfig = new List<string>
        {
            "",
            "[Peer]",
            $"PublicKey = {peer.PublicKey}",
            $"AllowedIPs = {peer.AllowedIps}"
        };
        
        File.AppendAllLines(_configPath, peerConfig);

        RestartWg();
    }

    public void RemovePeer(string publicKey)
    {
        // Читаем весь конфиг
        string configContent = File.ReadAllText(_configPath);
        
        // Регулярное выражение для поиска секции пира
        string pattern = $@"(\[Peer\][^\[]*?PublicKey\s*=\s*{Regex.Escape(publicKey)}[^\[]*)";
        
        // Удаляем секцию пира
        string newConfig = Regex.Replace(
            configContent,
            pattern,
            "",
            RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        // Сохраняем изменения
        File.WriteAllText(_configPath, newConfig, Encoding.UTF8);
    }

    private string GeneratePrivateKey()
    {
        // Генерация приватного ключа (например, через `wg genkey`)
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "wg",
                Arguments = "genkey",
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };
        process.Start();
        return process.StandardOutput.ReadToEnd().Trim();
    }

    private string GeneratePublicKey(string privateKey)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "wg",
                Arguments = "pubkey",
                RedirectStandardInput = true,  // Приватный ключ передаётся в STDIN
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };
        process.Start();
        process.StandardInput.WriteLine(privateKey);  // Передаём приватный ключ
        process.StandardInput.Close();
        return process.StandardOutput.ReadToEnd().Trim();  // Получаем публичный ключ
    }

    private void RestartWg()
    {
        if (OperatingSystem.IsLinux())
            Process.Start("/bin/bash", "-c \"sudo systemctl restart wg-quick@wg0\"");
        else
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "net",
                    Arguments = "stop WireGuardManager && net start WireGuardManager",
                    Verb = "runas", // Запуск от имени администратора (обязательно!)
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true
                }
            };
            process.Start();
        }
    }
}