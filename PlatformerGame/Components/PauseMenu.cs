using Nez;
using Nez.UI;
using PlatformerGame.Helpers;
using PlatformerGame.Scenes;
using System.Collections.Generic;

namespace PlatformerGame.Components
{
    /// <summary>
    /// PauseMenu class that provides a simple way to pause the game
    /// </summary>
    internal class PauseMenu : BasicUI
    {
        // List of buttons in the PauseMenu
        private List<Button> _buttons;

        // Boolean to check if the game is paused
        public bool _paused;

        // GameData object to store the game data
        public GameData gameData;

        // SaveManager object to save the game
        private SaveManager saveManager;

        // LoadMenu object to load the game
        private LoadMenu loadMenu;

        // SettingsMenu object to change the game settings
        private SettingsMenu settingsMenu;

        /// <summary>
        /// Creates a new PauseMenu object with a UICanvas, paused boolean, and GameData object
        /// </summary>
        /// <param name="uiCanvas"></param>
        /// <param name="paused"></param>
        /// <param name="gameData"></param>
        public PauseMenu(UICanvas uiCanvas, bool paused, GameData gameData) : base(uiCanvas, "Pause Menu", 3.0f)
        {
            this.gameData = gameData;

            // Initialize the list of buttons
            _buttons = new List<Button>();

            // Initialize the SaveManager
            saveManager = new SaveManager();

            // Resume
            _buttons.Add(AddButton("Resume", (button) => { Hide(); paused = false; }));

            // Save Game
            _buttons.Add(AddButton("Save Game", (button) => { saveManager.SaveGame(gameData); }));

            // Load Game Menu
            loadMenu = new LoadMenu(uiCanvas);
            loadMenu.Hide();
            _buttons.Add(AddButton("Load Game", (button) => { loadMenu.Display(); }));

            // Options Menu
            settingsMenu = new SettingsMenu(uiCanvas);
            settingsMenu.Hide();
            _buttons.Add(AddButton("Options", (button) => { settingsMenu.Display(); }));

            // Exit the game
            _buttons.Add(AddButton("Save & Exit", (button) => { saveManager.SaveGame(gameData); Core.StartSceneTransition(new FadeTransition(() => new MainMenuScene())); }));
        }

        /// <summary>
        /// Overrides the BasicUI hide method to hide the PauseMenu and the LoadMenu and SettingsMenu
        /// </summary>
        public override void Hide()
        {
            _table.RemoveElement(_window);

            loadMenu.Hide();
            settingsMenu.Hide();
        }
    }
}
