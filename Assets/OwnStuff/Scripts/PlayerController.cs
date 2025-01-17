using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerControls playerControls;
    [SerializeField]
    private ClientPacketSender clientPacketSender;

    [SerializeField]
    private ClientMovement clientMovement;

    private const float shootCoolDown = 0.25f;
    private float shootTimer = 0f;
    private Vector3 lastDirection = new Vector3(1, 0, 0);

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;
        Vector2 inputVec2 = playerControls.Playing.Movement.ReadValue<Vector2>();
        Vector3 input = new Vector3(inputVec2.x, inputVec2.y, 0).normalized;

        if (input != Vector3.zero)
        {
            lastDirection = input;
            clientMovement.MovePlayer(input);
        }

        bool shoot = playerControls.Playing.Shoot.IsPressed();
        if (shoot && shootTimer < 0f)
        {
            if (clientPacketSender)
            {
                clientPacketSender.ShootServerRpc(transform.position, lastDirection);
                shootTimer = shootCoolDown;
            }
            
        }
    }

    public void EnableController(bool b)
    {
        enabled = b;
        if (playerControls != null)
        {
            if (b)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }
}
