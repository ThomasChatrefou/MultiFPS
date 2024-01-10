using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button _serverButton;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;

    private void StartServer() => NetworkManager.Singleton?.StartServer();
    private void StartHost() => NetworkManager.Singleton?.StartHost();
    private void StartClient() => NetworkManager.Singleton?.StartClient();

    private void Awake()
    {
        _serverButton.onClick.AddListener(StartServer);
        _hostButton.onClick.AddListener(StartHost);
        _clientButton.onClick.AddListener(StartClient);
    }
}
