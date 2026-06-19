using Nez;
using PlatformerGame.Helpers;
using PlatformerGame.Scenes;
using System;

namespace PlatformerGame.Components
{
    /// <summary>
    /// WinGameMenu class that provides a simple way to navigate the game
    /// </summary>
    internal class WinGameMenu : BasicUI
    {
        /// <summary>
        /// Initializes the WinGameMenu
        /// </summary>
        /// <param name="uiCanvas"></param>
        public WinGameMenu(UICanvas uiCanvas) : base(uiCanvas, "You Win!", 3.0f)
        {
            // Load the game data
            var saveManager = new SaveManager();
            saveManager.GetCurrentID(out int saveID);
            saveManager.LoadGame(saveID, out var gameData);

            // Calculate the score
            gameData.Score = CalculateScore(gameData);

            // Display the score
            AddLabel($"You have finished the game! Your total score was: {gameData.Score}", 2);

            // Add the main menu and exit buttons
            AddButton("Main Menu", (button) => { saveManager.DeleteGame(gameData.SaveID); Core.StartSceneTransition(new FadeTransition(() => new MainMenuScene())); });
            AddButton("Exit", (button) => { saveManager.DeleteGame(saveID); Core.Exit(); });
        }

        /// <summary>
        /// Calculates the score based on the game data
        /// </summary>
        /// <param name="gameData"></param>
        /// <returns></returns>
        private int CalculateScore(GameData gameData)
        {
            return gameData.RoomsVisited * (Convert.ToInt32(Math.Pow(gameData.ItemScore, 2)) / Convert.ToInt32(gameData.TimePlayed));
        }
    }
}
