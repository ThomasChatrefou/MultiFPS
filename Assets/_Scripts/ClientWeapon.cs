using Unity.Netcode;
using UnityEngine;
using Unity.FPS.Game;

[RequireComponent(typeof(WeaponController))]
public class ClientWeapon : NetComponent
{
    [ClientRpc]
    public void ShootClientRpc(NetworkBehaviourReference[] projectileRefs)
    {
        EnsureHasStarted();

        foreach (NetworkBehaviourReference projectileRef in projectileRefs)
        {
            if (projectileRef.TryGet(out ProjectileBase projectile))
            {
                projectile.Shoot(_controller);
            }
            else
            {
                if (!_warnOnceFlag)
                {
                    DebugUtility.HandleErrorWithCustomMessage(
                    message: "Did not find " + nameof(ProjectileBase) + " out of " + nameof(NetworkBehaviourReference) + " in " + nameof(ShootClientRpc),
                    source: this);
                    _warnOnceFlag = true;
                }
            }
        }

        _controller.TryPlayShootSFX();
        _controller.TryTriggerAttackAnimation();
    }

    protected override void NetStart()
    {
        _controller = GetComponent<WeaponController>();
    }

    private WeaponController _controller;
    private bool _warnOnceFlag = false;
}