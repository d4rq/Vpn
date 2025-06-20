using Vpn.Core.Models;

namespace Vpn.Core.Abstractions;

public interface IPeerService
{
    List<PeerInfo> GetPeers();
    void AddPeer(PeerInfo peer);
    void RemovePeer(string publicKey);
}