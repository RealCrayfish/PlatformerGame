using Nez;

namespace PlatformerGame.Components
{
    /// <summary>
    /// Teleporter component that teleports the player to another location when they enter the trigger.
    /// </summary>
    public class Teleporter : Component, ITriggerListener
    {
        #region ITriggerListener implementation

        /// <summary>
        /// Triggered when a collider enters the teleporter. Used to teleport the player.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="local"></param>
        public void OnTriggerEnter(Collider other, Collider local)
        {
            // Check if the other collider has a Player component
            var playerComponent = other.Entity.GetComponent<Player>();
            if (playerComponent != null)
            {
                // Log teleportation event
                Debug.Log($"Teleporting player via teleporter: {Entity.Name}");

                // Trigger teleportation
                playerComponent.TriggerTeleport(Entity.Name);
            }
            else
            {
                // Log if the player component is not found
                Debug.Log("Player component not found on entity entering teleporter.");
            }
        }

        // Not used, but required by ITriggerListener
        public void OnTriggerExit(Collider other, Collider local) { }

        #endregion
    }
}
