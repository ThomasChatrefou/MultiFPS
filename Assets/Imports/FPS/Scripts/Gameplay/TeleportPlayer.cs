using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    // Debug script, teleports the player across the map for faster testing
    public class TeleportPlayer : NetComponent
    {
        public KeyCode ActivateKey = KeyCode.F12;

        PlayerCharacterController m_PlayerCharacterController;

        protected override void NetStart()
        {
            m_PlayerCharacterController = FindObjectOfType<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, TeleportPlayer>(
                m_PlayerCharacterController, this);
        }

        protected override void NetUpdate()
        {
            if (Input.GetKeyDown(ActivateKey))
            {
                m_PlayerCharacterController.transform.SetPositionAndRotation(transform.position, transform.rotation);
                Health playerHealth = m_PlayerCharacterController.GetComponent<Health>();
                if (playerHealth)
                {
                    playerHealth.Heal(999);
                }
            }
        }

    }
}