using UnityEngine;
using Unity.Netcode;

public class ExtendedNetworkManager : NetworkManager
{
    [SerializeField]
    private ServerManager serverManagerPrefab;
    public new bool StartHost()
    {
        bool result = base.StartHost();
        Debug.Log("Testing with overriding");
        return result;
    }


}
