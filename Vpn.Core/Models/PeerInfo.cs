using System.ComponentModel.DataAnnotations;

namespace Vpn.Core.Models;

public class PeerInfo
{
    /// <summary>
    /// Публичный ключ пира WG
    /// </summary>
    public string PublicKey { get; set; }
    
    /// <summary>
    /// Допустимые адреса
    /// </summary>
    public string AllowedIps { get; set; }
}