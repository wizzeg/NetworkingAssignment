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
    public HitPlayerDelegate HitPlayer; // 

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
            // move bullet, Vector3.Translate doesn't want to work
            float dt = Time.deltaTime;
            timer += dt;
            transform.position += speed * dt * direction;

            // server will despawn bullets that have existed for too long
            if (timer > lifetime)
            {
                DespawnBullet(GetComponent<NetworkObject>());
            }

            // server does a hit detection, for each bullet only on players
            Collider[] hits = Physics.OverlapSphere(transform.position, 0.3f, LayerMask.GetMask("Player"));
            foreach (var hit in hits)
            {
                // if bullet hit a player it'll just fetch the clientID
                ulong clientID = hit.gameObject.GetComponent<ClientPacketSender>().GetClientID();
                if (!clientID.Equals(ownerID))
                {
                    HitPlayer(clientID, GetComponent<NetworkObject>(), transform.position, -direction);
                }

            }
            
        }
    }
}
