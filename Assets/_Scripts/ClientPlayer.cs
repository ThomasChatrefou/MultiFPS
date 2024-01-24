using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using Unity.Netcode;
using UnityEngine;

public class ClientPlayer : NetComponent
{
    [ClientRpc]
    public void SpawnWeaponClientRpc(NetworkObjectReference weaponRef, NetworkBehaviourReference controllerRef, int slot, ClientRpcParams rpcParams)
    {
        EnsureHasStarted();

        NetworkObject weapon = weaponRef;
        if (controllerRef.TryGet(out WeaponController controller))
        {
            // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
            controller.Owner = gameObject;
            controller.SourcePrefab = weapon.gameObject;
            controller.ShowWeapon(false);

            // Assign the first person layer to the weapon. This function converts a layermask to a layer index
            int layerIndex = Mathf.RoundToInt(Mathf.Log(_weaponsManager.FpsWeaponLayer.value, 2));
            foreach (Transform t in weapon.gameObject.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = layerIndex;
            }

            _weaponsManager.FillWeaponSlot(controller, slot);
            _weaponsManager.OnAddedWeapon?.Invoke(controller, slot);
        }
        else
        {
            DebugUtility.HandleErrorWithCustomMessage(
                message: "Did not find " + nameof(WeaponController) + " out of " + nameof(NetworkBehaviourReference) + " in " + nameof(SpawnWeaponClientRpc),
                source: this);
        }
    }

    protected override void NetStart()
    {
        _weaponsManager = GetComponent<PlayerWeaponsManager>();
    }

    private PlayerWeaponsManager _weaponsManager;
}
