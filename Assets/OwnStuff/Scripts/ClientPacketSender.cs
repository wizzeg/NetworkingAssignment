using Unity.Netcode;
using UnityEngine;

public class ClientPacketSender : NetworkBehaviour
{
    // This is where the networking for the players would really appear, normally not everything has to be done with networking
    [ServerRpc]
    public void ShootServerRpc(Vector3 origin, Vector3 direction)
    {
        // just shoot a bullet, ServerManager doesn't exist on clients, but it does on server
        if (ServerManager.instance != null)
        {
            ServerManager.instance.PlayerShoot(OwnerClientId, origin, direction);
        }
    }

    public ulong GetClientID()
    {
        return OwnerClientId;
    }
}
