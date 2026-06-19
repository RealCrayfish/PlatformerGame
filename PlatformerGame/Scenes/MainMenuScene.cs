using Microsoft.Xna.Framework;
using Nez;
using PlatformerGame.Components;
using PlatformerGame.Helpers;

namespace PlatformerGame.Scenes;

/// <summary>
/// Main Menu class that provides a simple way to navigate the game
/// </summary>
internal class MainMenuScene : Scene
{
    // UICanvas for the MainMenu
    private UICanvas uiCanvas;
    private MainMenu mainMenu;

    /// <summary>
    /// Initializes the MainMenuScene
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        ClearColor = new Color(14, 7, 27, 255);
        uiCanvas = CreateEntity("ui").AddComponent(new UICanvas());

        // Create a new MainMenu object
        mainMenu = new MainMenu(uiCanvas, new GameData());
        mainMenu.Display();
    }

    /// <summary>
    /// Updates the MainMenuScene
    /// </summary>
    public override void Update()
    {
        base.Update();
    }
}

