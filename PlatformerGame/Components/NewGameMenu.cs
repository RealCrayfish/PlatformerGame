using Nez;
using Nez.UI;
using PlatformerGame.Helpers;
using System.Collections.Generic;

namespace PlatformerGame.Components;

/// <summary>
/// New game menu.
/// </summary>
internal class NewGameMenu : BasicUI
{
    private UICanvas uiCanvas;

    // List of buttons in the menu.
    private List<Button> _buttons;

    // Game data and save manager.
    private GameData gameData;
    private SaveManager saveManager;

    // Text field for player name.
    private TextField _textField;

    /// <summary>
    /// Creates a new NewGameMenu object with a title and font scale.
    /// </summary>
    /// <param name="uiCanvas"></param>
    public NewGameMenu(UICanvas uiCanvas) : base(uiCanvas, "Main Menu", 3.0f)
    {
        this.uiCanvas = uiCanvas;

        // Initialize list of buttons and save manager.
        _buttons = new List<Button>();
        saveManager = new SaveManager();

        // Add text field to the window.
        _textField = AddTextField("Enter your name");
        _textField.SetText("Player");

        // Add buttons to the window.
        _buttons.Add(AddButton("Start", (button) => StartGame(_textField.GetText())));
        _buttons.Add(AddButton("Back", (button) => Hide()));
    }

    /// <summary>
    /// Click event for starting the game.
    /// </summary>
    /// <param name="playerName"></param>
    private void StartGame(string playerName)
    {
        // Check for valid name.
        if (string.IsNullOrEmpty(playerName.Trim()))
        {
            Debug.Log("Please enter a valid name");
            _textField.SetText("Player");
            return;
        }

        // Create new game data and switch scene.
        if (saveManager.NewSavegame(playerName.Trim()))
        {
            saveManager.StartGame();
        }

    }
}
