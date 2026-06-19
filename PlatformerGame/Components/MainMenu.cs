using Nez;
using Nez.UI;
using PlatformerGame.Helpers;
using System.Collections.Generic;

namespace PlatformerGame.Components;

/// <summary>
/// Main Menu class that provides a simple way to navigate the game
/// </summary>
internal class MainMenu : BasicUI
{
    private UICanvas uiCanvas;

    // List of buttons in the MainMenu
    private List<Button> _buttons;

    // GameData and SaveManager
    private GameData gameData;
    private SaveManager saveManager;

    /// <summary>
    /// Creates a new MainMenu object with a UICanvas and GameData
    /// </summary>
    /// <param name="uiCanvas"></param>
    /// <param name="gameData"></param>
    public MainMenu(UICanvas uiCanvas, GameData gameData) : base(uiCanvas, "Main Menu", 3.0f)
    {
        this.uiCanvas = uiCanvas;
        this.gameData = gameData;

        // Initialize the list of buttons
        _buttons = new List<Button>();

        // Initialize the SaveManager
        saveManager = new SaveManager();

        // New Game Menu
        NewGameMenu newGameMenu = new NewGameMenu(uiCanvas);
        newGameMenu.Hide();
        _buttons.Add(AddButton("New Game", (button) => { newGameMenu.Display(); }));

        // If there is a current save file, add a continue game button
        if (saveManager.GetCurrentID(out int saveID))
            _buttons.Add(AddButton("Continue Game", (button) => { saveManager.StartGame(); }));

        // Load Game Menu
        LoadMenu loadMenu = new LoadMenu(uiCanvas);
        loadMenu.Hide();
        _buttons.Add(AddButton("Load Game", (button) => { loadMenu.Display(); }));

        // Options Menu
        SettingsMenu settingsMenu = new SettingsMenu(uiCanvas);
        settingsMenu.Hide();
        _buttons.Add(AddButton("Options", (button) => { settingsMenu.Display(); }));

        // Exit the game
        _buttons.Add(AddButton("Exit", (button) => { Core.Exit(); }));
    }
}

