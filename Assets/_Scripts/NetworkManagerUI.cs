using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{

    private void StartServer() => StartSession(NetworkManager.Singleton.StartServer, doOnce: true);
    private void StartHost() => StartSession(NetworkManager.Singleton.StartHost, doOnce: true);
    private void StartClient() => StartSession(NetworkManager.Singleton.StartClient, doOnce: true);
    private void OnEnable()
    {
        _serverButton.onClick.AddListener(StartServer);
        _hostButton.onClick.AddListener(StartHost);
        _clientButton.onClick.AddListener(StartClient);
    }

    private void OnDisable()
    {
        _serverButton.onClick.RemoveListener(StartServer);
        _hostButton.onClick.RemoveListener(StartHost);
        _clientButton.onClick.RemoveListener(StartClient);
        _startersDone.Clear();
    }

    private delegate bool Starter();

    private void StartSession(Starter starterCall, bool doOnce = false)
    {
        if (doOnce)
        {
            if (_startersDone.Contains(starterCall))
            {
                return;
            }
            _startersDone.Add(starterCall);
        }
        starterCall();
    }

    [SerializeField] private Button _serverButton;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;

    [NonSerialized]
    private HashSet<Starter> _startersDone = new();
}
