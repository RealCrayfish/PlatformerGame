using Nez;

namespace PlatformerGame.Components
{
    /// <summary>
    /// Component that adds an upgrade to the player when collided with
    /// </summary>
    public class Upgrade : Component, ITriggerListener
    {
        #region ITriggerListener implementation

        /// <summary>
        /// Called when a collider intersects a trigger collider. Calls the methods to add an upgrade to the player
        /// </summary>
        /// <param name="other"></param>
        /// <param name="local"></param>
        public void OnTriggerEnter(Collider other, Collider local)
        {
            // Adds an upgrade to the player if collided with and destroys the upgrade
            if (Entity.Name == "staircase")
            {
                other.Entity.Components.GetComponent<Player>(true).AddStaircase();
            }
            else
            {
                other.Entity.Components.GetComponent<Player>(true).AddItemScore(Entity.Name);
            }
            Entity.Destroy();
        }

        // Not used, but required for ITriggerListener
        public void OnTriggerExit(Collider other, Collider local)
        { }

        #endregion
    }
}
