using Unity.FPS.Game;
using Unity.Netcode;
using UnityEngine;

public struct SpawnData<COMPONENT> where COMPONENT : Component
{
    public const float INFINITE_LIFETIME = -1f;

    public COMPONENT Prefab;    // could become a list if necessary => then take care of NetworkSpawnHelper.SpawnNetObject(SpawnData data)
    public Transform Anchor;
    public float Lifetime;
    public bool IsChildOfAnchor;
}

public interface ISpawner<COMPONENT> where COMPONENT : Component
{
    public SpawnData<COMPONENT> SpawningData { get; }
}

public class NetworkSpawnHelper
{
    public static COMPONENT SpawnNetObject<COMPONENT>(SpawnData<COMPONENT> data) where COMPONENT : Component
    {
        return SpawnNetObject(data.Prefab, data.Anchor.position, data.Anchor.rotation, data.IsChildOfAnchor ? data.Anchor : null, data.Lifetime);
    }

    public static COMPONENT SpawnNetObject<COMPONENT>(COMPONENT prefab, Vector3 position, Quaternion orientation, Transform parent = null, float lifetime = SpawnData<COMPONENT>.INFINITE_LIFETIME)
        where COMPONENT : Component
    {
        GameObject newObject = SpawnNetObject(prefab.gameObject, position, orientation, parent, lifetime);
        return newObject.GetComponent<COMPONENT>();
    }

    public static GameObject SpawnNetObject(GameObject prefab, Vector3 position, Quaternion orientation, Transform parent, float lifetime)
    {
        GameObject newObject = Object.Instantiate(prefab, position, orientation, parent);
        if (!newObject.TryGetComponent(out NetworkObject netObject))
        {
            DebugUtility.HandleErrorIfNoComponentFound_StrSource<NetworkObject>(count: 0,
                source: nameof(NetworkSpawnHelper) + "." + nameof(SpawnNetObject),
                onObject: newObject);
            return null;
        }
        netObject.Spawn(destroyWithScene: true);
        NetworkObject netParent = parent.GetComponentInParent<NetworkObject>();
        netObject.TrySetParent(netParent);

        // Warning ! Should take care of sync between players
        if (lifetime > 0f)
        {
            Object.Destroy(newObject, lifetime);
        }

        return newObject;
    }
}
