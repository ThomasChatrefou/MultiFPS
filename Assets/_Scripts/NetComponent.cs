using Unity.Netcode;

public class NetComponent : NetworkBehaviour
{
    protected virtual void NetStart() { }

    protected virtual void NetUpdate() { }

    protected virtual void NetLateUpdate() { }

    protected virtual void NetUnload() { }

    protected void EnsureHasStarted()
    {
        if (!_hasStarted)
        {
            OnStart();
        }
    }

    private void Start()
    {
        _networkManager = NetworkManager.Singleton;

        if (IsSpawned)
        {
            OnStart();
        }
        else
        {
            _networkManager.OnServerStarted += OnStart;
        }
    }

    private void OnStart()
    {
        if (!IsOwner) return;

        _hasStarted = true;
        NetStart();
    }

    private void Update()
    {
        if (!IsOwner) return;

        NetUpdate();
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;

        NetLateUpdate();
    }

    private void OnDisable()
    {
        if (!IsSpawned)
        {
            _networkManager.OnServerStarted -= NetStart;
        }
    }

    private NetworkManager _networkManager;
    private bool _hasStarted = false;
}
