using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// A basic example of a UI to start a host or client.
/// If you want to modify this Script please copy it into your own project and add it to your copied UI Prefab.
/// </summary>
public class TempUI : MonoBehaviour
{
    [SerializeField]
    Button m_StartHostButton;
    [SerializeField]
    Button m_StartClientButton;
    [SerializeField]
    ServerManager serverManager;
    [SerializeField]
    GameObject serverManagerPrefab;
    private GameObject serverManagerInstance;

    void Awake()
    {
        if (!FindAnyObjectByType<EventSystem>())
        {
            var inputType = typeof(StandaloneInputModule);
#if ENABLE_INPUT_SYSTEM && NEW_INPUT_SYSTEM_INSTALLED
            inputType = typeof(InputSystemUIInputModule);                
#endif
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
            eventSystem.transform.SetParent(transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_StartHostButton.onClick.AddListener(StartHost);
        m_StartClientButton.onClick.AddListener(StartClient);
    }

    void StartClient()
    {
        //if (serverManager == null || serverManager.CallBacksSet == false) return; // Order matters, should be ok in C#
        NetworkManager.Singleton.StartClient();
        DeactivateButtons();
    }

    void StartHost()
    {
        // if (serverManager == null || serverManager.CallBacksSet == false) return; // Order matters, should be ok in C#

        if (ServerManager.instance == null)
        {
            GameObject serverManagerInstance = Instantiate(serverManagerPrefab);
            serverManager = GetComponent<ServerManager>();
        }
        NetworkManager.Singleton.StartHost();
        DeactivateButtons();
    }

    void DeactivateButtons()
    {
        m_StartHostButton.interactable = false;
        m_StartClientButton.interactable = false;
        this.gameObject.SetActive(false);
    }
}