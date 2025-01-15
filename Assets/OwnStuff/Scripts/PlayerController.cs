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

    private const float shootCoolDown = 3f;
    private float shootTimer = 3f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        shootTimer -= Time.deltaTime;
        Vector2 inputVec2 = playerControls.Playing.Movement.ReadValue<Vector2>();
        Vector3 input = new Vector3(inputVec2.x, inputVec2.y, 0).normalized;

        if (input != Vector3.zero)
        {
            clientMovement.MovePlayer(input);
        }

        bool shoot = playerControls.Playing.Shoot.IsPressed();
        if (shoot && shootTimer < 0f)
        {
            Debug.Log("I am client and I want to shoot");
            if (clientPacketSender)
            {
                clientPacketSender.ShootServerRpc();
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
