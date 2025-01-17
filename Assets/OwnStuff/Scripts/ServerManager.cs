using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using System.Collections;

public class ServerManager : MonoBehaviour
{
    // This will only exist on the server

    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    GameObject explosionPrefab;

    public bool CallBacksSet { protected set; get;}
    public static ServerManager instance;

    public void Awake()
    {
        instance = this;
        StartCoroutine(SetCallBacks());
    }

    public void OnEnable()
    {
        // The network manager might not exist yet, so we just do a coroutine
        if (!CallBacksSet)
        {
            StartCoroutine(SetCallBacks());
        }
        
    }

    private void ServerManager_OnClientConnected(ulong playerId)
    {
        // Spawning player manually, because it was better leraning exerpience
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId, true);
        }
    }


    private void ServerManager_OnClientDisconnect(ulong playerId)
    {
        // not needed
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
                // NetworkManager.Singleton.OnClientDisconnectCallback += ServerManager_OnClientDisconnect;
                CallBacksSet = true;
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
        // instantiate a bullet, and set the networkvariables, and spawn it.
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = origin;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        // players won't get this data, but only the server needs it anyway
        BulletMove tempBulletMove = bullet.GetComponent<BulletMove>();
        tempBulletMove.direction = direction;
        tempBulletMove.ownerID = clientID;
        tempBulletMove.HitPlayer += HitPlayer;
        tempBulletMove.DespawnBullet += DespawnNetworkObject;

        bullet.GetComponent<NetworkObject>().Spawn(true);
    }

    public void HitPlayer(ulong clientID, NetworkObject bullet, Vector3 position, Vector3 direction)
    {
        // playet was hit, so we must create an explosion with the data we were given.
        position -= 0.25f * direction;
        GameObject explosion = Instantiate(explosionPrefab);

        explosion.GetComponent<NetworkObject>().Spawn();
        // players won't get all this data, but the NetworkVariables data will still shared to the players
        BulletExplodeSync tempBulletExplodeSync = explosion.GetComponent<BulletExplodeSync>();
        tempBulletExplodeSync.bulletExplodeSync += DespawnNetworkObject; // new players won't get this data, but only server needs it anyways
        tempBulletExplodeSync.SetBulletData(position, direction); // modifying NetworkVariables must apparently be done after it has spawned, makes sense I guess
        DespawnNetworkObject(bullet);
    }

    public void DespawnNetworkObject(NetworkObject networkObject)
    {
        // simply despawn the object
        if (networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
    }
}
