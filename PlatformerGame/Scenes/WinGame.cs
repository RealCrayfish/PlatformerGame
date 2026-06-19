using Nez;
using Nez.UI;
using PlatformerGame.Components;

namespace PlatformerGame.Scenes
{
    /// <summary>
    /// WinGame class that provides a simple way to create a win game scene
    /// </summary>
    internal class WinGame : Scene
    {
        // UI Variables
        public Table WinGameTable;

        /// <summary>
        /// Creates a new WinGame object
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Create UI Canvas
            var uiCanvas = CreateEntity("ui").AddComponent(new UICanvas());

            // Create WinGame menu
            WinGameMenu winGameMenu = new WinGameMenu(uiCanvas);
            winGameMenu.Display();
        }
    }
}

