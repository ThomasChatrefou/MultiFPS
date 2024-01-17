using Unity.Netcode;

public class NetComponent : NetworkBehaviour
{
    protected virtual void NetStart() { }

    protected virtual void NetUpdate() { }

    protected virtual void NetUnload() { }

    private void Start()
    {
        _networkManager = NetworkManager.Singleton;

        if (IsSpawned)
        {
            OnSpawn();
        }
        else
        {
            _networkManager.OnServerStarted += OnSpawn;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        NetUpdate();
    }

    private void OnDisable()
    {
        if (!IsSpawned)
        {
            _networkManager.OnServerStarted -= NetStart;
        }
    }

    private void OnSpawn()
    {
        if (!IsOwner) return;
        NetStart();
    }

    private NetworkManager _networkManager;
}
