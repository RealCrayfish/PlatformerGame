using Microsoft.Xna.Framework;
using Nez;
using PlatformerGame.Helpers;

namespace PlatformerGame
{
    /// <summary>
    /// The entry point for the game
    /// </summary>
    public class Game1 : Core
    {
        public Game1() { }

        // GameData object
        public GameData gameData;

        /// <summary>
        /// Initializes the game
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Set game properties
            ExitOnEscapeKeypress = false;
            PauseOnFocusLost = true;
            IsFixedTimeStep = true;

            // Loads the main menu scene
            Scene = new Scenes.MainMenuScene();
            Scene.ClearColor = new Color(14, 7, 27, 255);
        }
    }
}
