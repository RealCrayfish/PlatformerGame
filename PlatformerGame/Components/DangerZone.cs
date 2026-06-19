using Nez;

namespace PlatformerGame.Components
{
    /// <summary>
    /// DangerZone component that kills the player when they enter it
    /// </summary>
    public class DangerZone : Component, ITriggerListener
    {
        #region ITriggerListener implementation


        /// <summary>
        /// Sets the player to dead when they enter the danger zone
        /// </summary>
        /// <param name="other"></param>
        /// <param name="local"></param>
        public void OnTriggerEnter(Collider other, Collider local)
        {
            other.Entity.Components.GetComponent<Player>(true).KillPlayer();
        }

        // Not used, but required for ITriggerListener
        public void OnTriggerExit(Collider other, Collider local)
        { }

        #endregion
    }
}
