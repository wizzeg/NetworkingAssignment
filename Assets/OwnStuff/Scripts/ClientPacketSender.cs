using Unity.Netcode;
using UnityEngine;

public class ClientPacketSender : NetworkBehaviour
{
    [ServerRpc]
    public void ShootServerRpc()
    {
        ServerManager.instance.PlayerShoot(OwnerClientId);
    }
}
