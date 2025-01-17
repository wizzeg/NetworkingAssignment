using Unity.Netcode;
using UnityEngine;

public class BulletExplodeSync : NetworkBehaviour
{
    // Purpose of object is to be a reference for players on where a hit was detected, so they can play their little particle effect
    // This allows players who joined after hits to also play the animation

    public NetworkVariable<Vector3> origin = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector3> direction = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField]
    private GameObject bulletExplosionPrefab;

    private bool hasExploded = false;
    private float timer = 0f;
    private float lifeTime = 1.5f;

    private GameObject bulletExplode;

    public delegate void BulletExplodeSyncDelegate(NetworkObject explosionSync);
    public BulletExplodeSyncDelegate bulletExplodeSync;
    private void Update()
    {
        timer += Time.deltaTime;
        // ensures that lookrotation isn't zero, so that the explosion doesn't start at wrong position and rotation
        // this also removes the need to subscribe to "OnValueChange", because it has already changed for this to pass
        if (!hasExploded && direction.Value != Vector3.zero && IsClient) 
        {
            bulletExplode = Instantiate(bulletExplosionPrefab); // simply spawn the explosion locally
            bulletExplode.transform.position = origin.Value;
            bulletExplode.transform.rotation = Quaternion.LookRotation(direction.Value);
            hasExploded = true;
        }
        // Destroy this object after it's lived long enough, by server
        if (IsServer && timer > lifeTime)
        {
            bulletExplodeSync(GetComponent<NetworkObject>());
        }
    }
    public void SetBulletData(Vector3 origin, Vector3 direction)
    {
        // sets the data necessary to know where to play the particle system locally
        if (IsServer)
        {
            this.origin.Value = origin;
            this.direction.Value = direction;
        }
    }
}

