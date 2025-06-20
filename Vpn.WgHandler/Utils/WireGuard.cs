using System.Diagnostics;

namespace Vpn.WgHandler.Utils;

public static class WireGuard
{
    public static void EnsureWgInstalled()
    {
        if (OperatingSystem.IsLinux())
            EnsureInstalledOnLinux();
        else EnsureInstalledOnWindows();
    }

    private static void EnsureInstalledOnLinux()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "which",
                Arguments = "wg",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var installationProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sudo",
                    Arguments = "apt update && sudo apt install wireguard -y",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            installationProcess.Start();
            installationProcess.WaitForExit();
        }
    }

    private static void EnsureInstalledOnWindows()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C where wg",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var installationProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = "install WireGuard.WireGuard",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            installationProcess.Start();
            installationProcess.WaitForExit();
        }
    }
}