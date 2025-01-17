using Unity.Netcode;
using UnityEngine;

public class BulletMove : NetworkBehaviour
{
    [SerializeField]
    float speed = 30f;
    private float lifetime = 5f;
    private float timer = 0f;
    public ulong ownerID;
    public Vector3 direction = Vector3.zero;
    public delegate void HitPlayerDelegate(ulong clientID, NetworkObject bullet, Vector3 position, Vector3 direction);
    public HitPlayerDelegate HitPlayer;
    public delegate void DespawnBulletDelegate(NetworkObject bullet);
    public DespawnBulletDelegate DespawnBullet;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            float dt = Time.deltaTime;
            timer += dt;
            transform.position += speed * dt * direction;
            if (timer > lifetime)
            {
                DespawnBullet(GetComponent<NetworkObject>());
            }
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.3f, LayerMask.GetMask("Player"));
            foreach (var hit in hits)
            {
                ulong clientID = hit.gameObject.GetComponent<ClientPacketSender>().GetClientID();
                if (!clientID.Equals(ownerID))
                {
                    HitPlayer(clientID, GetComponent<NetworkObject>(), transform.position, -direction);
                }

            }
            
        }
    }
}
