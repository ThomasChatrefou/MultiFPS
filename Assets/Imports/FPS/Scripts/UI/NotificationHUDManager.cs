using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class NotificationHUDManager : NetComponent
    {
        [Tooltip("UI panel containing the layoutGroup for displaying notifications")]
        public RectTransform NotificationPanel;

        [Tooltip("Prefab for the notifications")]
        public GameObject NotificationPrefab;

        protected override void NetStart()
        {
            PlayerWeaponsManager playerWeaponsManager = FindObjectOfType<PlayerWeaponsManager>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerWeaponsManager, NotificationHUDManager>(playerWeaponsManager,
                this);
            playerWeaponsManager.OnAddedWeapon += OnPickupWeapon;

            EventManager.AddListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);
        }

        void OnObjectiveUpdateEvent(ObjectiveUpdateEvent evt)
        {
            if (!string.IsNullOrEmpty(evt.NotificationText))
                CreateNotification(evt.NotificationText);
        }

        void OnPickupWeapon(WeaponController weaponController, int index)
        {
            if (index != 0)
                CreateNotification("Picked up weapon : " + weaponController.WeaponName);
        }

        public void CreateNotification(string text)
        {
            GameObject notificationInstance = Instantiate(NotificationPrefab, NotificationPanel);
            notificationInstance.transform.SetSiblingIndex(0);

            NotificationToast toast = notificationInstance.GetComponent<NotificationToast>();
            if (toast)
            {
                toast.Initialize(text);
            }
        }

        public override void OnDestroy()
        {
            EventManager.RemoveListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);
        }
    }
}