using Unity.Netcode;
using UnityEngine;
using Unity.FPS.Game;

[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(ClientWeapon))]
public class ServerWeapon : NetComponent
{
    [ServerRpc]
    public void ShootServerRpc(int projectileCount)
    {
        EnsureHasStarted();

        Transform muzzle = _controller.WeaponMuzzle;

        ProjectileBase[] projectiles = new ProjectileBase[projectileCount];
        for (int i = 0; i < projectileCount; i++)
        {
            Vector3 shotDirection = _controller.GetShotDirectionWithinSpread(muzzle);
            projectiles[i] = NetworkSpawnHelper.SpawnNetObject(_controller.ProjectilePrefab, muzzle.position, Quaternion.LookRotation(shotDirection));
        }

        NetworkSpawnHelper.SpawnNetObject(_controller.MuzzleFlashPrefab, muzzle.position, muzzle.rotation, muzzle, lifetime: 2f);

        NetworkBehaviourReference[] projectileRefs = new NetworkBehaviourReference[projectileCount];
        for (int i = 0; i < projectileCount; i++)
        {
            projectileRefs[i] = projectiles[i];
        }
        
        _client.ShootClientRpc(projectileRefs);
    }

    protected override void NetStart()
    {
        _controller = GetComponent<WeaponController>();
        _client = GetComponent<ClientWeapon>();
    }

    private WeaponController _controller;
    private ClientWeapon _client;
}