using UnityEngine;
using Unity.Netcode;

public class ExtendedNetworkManager : NetworkManager
{
    public new bool StartHost()
    {
        bool result = base.StartHost();
        Debug.Log("Testing with overriding");
        return result;
    }
}
