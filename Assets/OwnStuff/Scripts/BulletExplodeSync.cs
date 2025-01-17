using Unity.Netcode;
using UnityEngine;

public class BulletExplodeSync : NetworkBehaviour
{
    public NetworkVariable<Vector3> origin = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector3> direction = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField]
    private GameObject bulletExplosionPrefab;
    private bool hasExploded = false;
    private float timer = 0f;
    private float lifeTime = 1.5f;

    GameObject bulletExplode;

    public delegate void BulletExplodeSyncDelegate(NetworkObject explosionSync);
    public BulletExplodeSyncDelegate bulletExplodeSync;

    private void OnEnable()
    {
        origin.OnValueChanged += OnOriginChanged;
        direction.OnValueChanged += OnDirectionChanged;
    }

    private void OnDisable()
    {
        origin.OnValueChanged -= OnOriginChanged;
        direction.OnValueChanged -= OnDirectionChanged;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (!hasExploded && direction.Value != Vector3.zero) // ensures that lookrotation isn't zero, this will make stuff look, until it's not zero
        {
            bulletExplode = Instantiate(bulletExplosionPrefab);
            bulletExplode.transform.position = origin.Value;
            bulletExplode.transform.rotation = Quaternion.LookRotation(direction.Value);
            hasExploded = true;
        }
        if (IsServer && timer > lifeTime)
        {
            Debug.Log("I should call for selfDestruction now...");
            bulletExplodeSync(GetComponent<NetworkObject>());
        }
    }
    public void SetBulletData(Vector3 origin, Vector3 direction)
    {
        if (IsServer)
        {
            Debug.Log("I'm server and I set bullet data");
            this.origin.Value = origin;
            this.direction.Value = direction;
        }
    }

    public Vector3 GetOrigin()
    {
        return (Vector3)this.origin.Value;
    }

    public Vector3 GetDirection()
    {
        return (Vector3)this.direction.Value;
    }

    private void OnOriginChanged(Vector3 oldValue, Vector3 newValue)
    {
        if (bulletExplode)
        {
            bulletExplode.transform.position = newValue;
        }
    }

    private void OnDirectionChanged(Vector3 oldValue, Vector3 newValue)
    {
        if (bulletExplode)
        {
            bulletExplode.transform.rotation = Quaternion.LookRotation(newValue);
        }
    }
}

