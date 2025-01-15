using Unity.Netcode;
using UnityEngine;

public class ClientOwner : NetworkBehaviour
{
    public PlayerController playerController;
    public PlayerControls playerControls;
    private void Awake()
    {   
        playerController.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log("Network spawned ClientOwner script");
        base.OnNetworkDespawn();
        enabled = IsClient;

        playerController.EnableController(IsOwner);
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
}
