using Unity.Netcode;
using UnityEngine;

public class ClientPacketSender : NetworkBehaviour
{
    [ServerRpc]
    public void ShootServerRpc(Vector3 origin, Vector3 direction)
    {
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
