using Unity.Netcode;
using UnityEngine;

public class RandomPlayerColor : NetworkBehaviour
{
    [SerializeField]
    Renderer renderer;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Random.InitState(((int)OwnerClientId));
        if (renderer != null)
        {
            renderer.material.color = Random.ColorHSV();
        }
            

    }
}
