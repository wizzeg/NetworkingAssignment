using Unity.Netcode;
using UnityEngine;

public class RandomPlayerColor : NetworkBehaviour
{
    [SerializeField]
    private Renderer renderer;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // "random" color, but the random seed is required to sync the random color between everyone
        Random.InitState(((int)OwnerClientId));
        if (renderer != null)
        {
            renderer.material.color = Random.ColorHSV();
        }
            

    }
}
