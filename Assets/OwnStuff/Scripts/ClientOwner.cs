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
        // Decideds if the player should be moved by input or not
        base.OnNetworkDespawn();
        enabled = IsClient;
        playerController.EnableController(IsOwner);
    }
}
