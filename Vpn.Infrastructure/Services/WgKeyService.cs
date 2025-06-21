using System.Diagnostics;

namespace Vpn.Infrastructure.Services;

public class WgKeyService
{
    public string GeneratePrivateKey()
    {
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

    public string GeneratePublicKey(string privateKey)
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
}