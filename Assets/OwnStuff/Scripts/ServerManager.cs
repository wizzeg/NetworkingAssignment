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
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = origin;
        bullet.transform.rotation = Quaternion.LookRotation(direction);

        BulletMove tempBulletMove = bullet.GetComponent<BulletMove>();
        tempBulletMove.direction = direction;
        tempBulletMove.HitPlayer += HitPlayer;
        tempBulletMove.ownerID = clientID;
        tempBulletMove.DespawnBullet += DespawnNetworkObject;

        bullet.GetComponent<NetworkObject>().Spawn(true);
    }

    public void HitPlayer(ulong clientID, NetworkObject bullet, Vector3 position, Vector3 direction)
    {
        position -= 0.25f * direction;
        Debug.Log("Client " + clientID + " was hit.");
        GameObject explosion = Instantiate(explosionPrefab);
        explosion.transform.position = position;
        explosion.transform.rotation = Quaternion.LookRotation(direction);

        explosion.GetComponent<NetworkObject>().Spawn();

        BulletExplodeSync tempBulletExplodeSync = explosion.GetComponent<BulletExplodeSync>();
        tempBulletExplodeSync.bulletExplodeSync += DespawnNetworkObject;
        tempBulletExplodeSync.SetBulletData(position, direction);
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
