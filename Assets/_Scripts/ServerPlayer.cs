using System.Collections.Generic;
using Unity.FPS.Game;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(ISpawner<WeaponController>))]
public class ServerPlayer : NetComponent
{
    [ServerRpc]
    public void SpawnWeaponServerRpc(ulong senderClientId, int slot)
    {
        EnsureHasStarted();

        WeaponController spawnedWeapon = NetworkSpawnHelper.SpawnNetObject(_weaponSpawner.SpawningData);

        _client.SpawnWeaponClientRpc(
            weaponRef: spawnedWeapon.gameObject,
            controllerRef: spawnedWeapon,
            slot,
            rpcParams: new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { senderClientId }
                }
            });
    }

    protected override void NetStart()
    {
        _weaponSpawner = GetComponent<ISpawner<WeaponController>>();
        _client = GetComponent<ClientPlayer>();
    }

    private ISpawner<WeaponController> _weaponSpawner;
    private ClientPlayer _client;
}
