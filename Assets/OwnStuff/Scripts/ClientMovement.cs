using Unity.Netcode;
using UnityEngine;

public class ClientMovement : NetworkBehaviour
{
    [SerializeField]
    private float speed = 5f;

    // client authorative movement
    public void MovePlayer(Vector3 input)
    {
        transform.position += speed * Time.deltaTime * input;
    }

}
