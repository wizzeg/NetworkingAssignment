using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using System.Collections;

public class ServerManager : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    GameObject explosionPrefab;

    private List<GameObject> players = new List<GameObject>();
    private List<ulong> playerIds = new List<ulong>();
    private List<GameObject> explosions = new List<GameObject>();
    public bool CallBacksSet { protected set; get;}

    public static ServerManager instance;

    public void Awake()
    {
        instance = this;
        StartCoroutine(SetCallBacks());
    }

    public void OnEnable()
    {
        if (!CallBacksSet)
        {
            StartCoroutine(SetCallBacks());
        }
        
    }

    private void ServerManager_OnClientConnected(ulong playerId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            playerIds.Add(playerId);
            Debug.Log("Spawning player on Server");
            GameObject player = Instantiate(playerPrefab);
            players.Add(player);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId, true);
        }
    }


    private void ServerManager_OnClientDisconnect(ulong playerId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            for (int i = 0; i < playerIds.Count; i++)
            {
                if (playerId.Equals(playerIds[i]))
                {
                    // should do proper despawn here
                    NetworkObject player = players[i].GetComponent<NetworkObject>();
                    if (player.IsSpawned)
                    {
                        player.Despawn();
                    }
                    players.RemoveAt(i);
                    playerIds.RemoveAt(i);
                    break;
                }
            }
        }

    }

    // Going to do this instead of Server Authoritative movement with prediction.
    private void StartMovementChecksSystem()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Starting System to check legal movement"); 
        }
        
    }

    public void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= ServerManager_OnClientConnected;
        }
    }

    private IEnumerator SetCallBacks()
    {
        int attempts = 0;
        while (!CallBacksSet && attempts < 1000)
        {
            if (NetworkManager.Singleton == null)
            {
                yield return null;
            }
            else
            {
                NetworkManager.Singleton.OnClientConnectedCallback += ServerManager_OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += ServerManager_OnClientDisconnect;
                CallBacksSet = true;
                Debug.Log("CallBacksSet");
            }
            attempts++;
        }
        if (!CallBacksSet)
        {
            Debug.Log("Failed to set CallBack");
        }
    }

    public void PlayerShoot(ulong clientID, Vector3 origin, Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = origin;
        bullet.transform.rotation = Quaternion.LookRotation(direction);
        bullet.GetComponent<BulletMove>().direction = direction;
        bullet.GetComponent<BulletMove>().HitPlayer += HitPlayer;
        bullet.GetComponent<BulletMove>().ownerID = clientID;
        bullet.GetComponent<BulletMove>().DespawnBullet += DespawnNetworkObject;
        //bullets.Add(bullet);
        bullet.GetComponent<NetworkObject>().Spawn(true);
        Debug.Log("Client " + clientID + " Shot");
    }

    public void HitPlayer(ulong clientID, NetworkObject bullet, Vector3 position, Vector3 direction)
    {
        position -= 0.25f * direction;
        Debug.Log("Client " + clientID + " was hit.");
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = position;
        explosion.transform.rotation = Quaternion.LookRotation(direction);
        explosion.GetComponent<NetworkObject>().Spawn();
        explosion.GetComponent<BulletExplodeSync>().bulletExplodeSync += DespawnNetworkObject;
        //explosion.GetComponent<BulletExplodeSync>().origin.Value = position;
        //explosion.GetComponent<BulletExplodeSync>().direction.Value = direction;
        explosion.GetComponent<BulletExplodeSync>().SetBulletData(position, direction);
        DespawnNetworkObject(bullet);
    }

    public void DespawnNetworkObject(NetworkObject networkObject)
    {
        if (networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
    }
}
