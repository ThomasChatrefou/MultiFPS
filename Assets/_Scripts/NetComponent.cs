using Unity.Netcode;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class NetComponent : MonoBehaviour
{
    protected virtual void NetStart() { }

    protected virtual void NetUpdate() { }

    protected virtual void NetUnload() { }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += NetStart;
    }

    private void Update()
    {
        if (NetworkManager.Singleton.IsApproved)
        {
            NetUpdate();
        }
    }

    // Trying to resolve networkManager events unsuscribing but it's too late
    // maybe it could be useful later

    //private void Awake()
    //{
    //    SceneManager.sceneUnloaded += OnSceneUnloaded;
    //}

    //private void OnSceneUnloaded(Scene scene)
    //{
    //    if (scene == base.gameObject.scene)
    //    {
    //        NetUnload();
    //        Debug.Log("component unloaded");
    //    }
    //}

    //private void OnDisable()
    //{
    //    NetUnload();
    //}
}
