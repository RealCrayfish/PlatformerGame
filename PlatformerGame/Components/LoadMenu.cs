using Nez;
using Nez.UI;
using PlatformerGame.Helpers;
using System.Collections.Generic;

namespace PlatformerGame.Components
{
    /// <summary>
    /// LoadMenu class that provides a simple way to load a game
    /// </summary>
    internal class LoadMenu : BasicUI
    {
        // List of buttons in the LoadMenu
        private List<Button> _buttons;

        /// <summary>
        /// Creates a new LoadMenu object with a UICanvas
        /// </summary>
        /// <param name="uiCanvas"></param>
        public LoadMenu(UICanvas uiCanvas) : base(uiCanvas, "Load Menu", 3.0f)
        {
            // Initialize the list of buttons
            _buttons = new List<Button>();

            // Add buttons for each save game.
            SaveManager saveManager = new SaveManager();
            List<GameData> saveGames = saveManager.ListSaves();
            foreach (GameData saveGame in saveGames)
            {
                _buttons.Add(AddButton($"{saveGame.Name}, Last Played: {saveGame.LastSaved}", (button) => { saveManager.SetCurrentID(saveGame.SaveID); saveManager.StartGame(); }));
            }

            // Back
            AddButton("Back", (button) => { Hide(); });
        }

        /// <summary>
        /// Displays the LoadMenu. Overrides the BasicUI Display method to update the list of save games.
        /// </summary>
        public override void Display()
        {
            base.Display();

            // Clear the window and buttons
            _window.Clear();
            _buttons.Clear();

            // Add the title and buttons
            AddLabel("Load Game", 3.0f);
            SaveManager saveManager = new SaveManager();
            List<GameData> saveGames = saveManager.ListSaves();
            foreach (GameData saveGame in saveGames)
            {
                _buttons.Add(AddButton($"{saveGame.Name}, Last Played: {saveGame.LastSaved}", (button) => { saveManager.SetCurrentID(saveGame.SaveID); saveManager.StartGame(); }));
            }

            // Back
            AddButton("Back", (button) => { Hide(); });
        }
    }
}
