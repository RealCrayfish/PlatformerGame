using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using Nez.UI;
using PlatformerGame.Components;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PlatformerGame.Helpers;

namespace PlatformerGame.Scenes;

/// <summary>
/// Represents a game scene in the PlatformerGame.
/// </summary>
abstract class GameScene : Scene
{
    #region Variables

    private bool _isPaused = false;
    private SaveManager _saveManager;
    private GameData _gameData;
    private float _timeSinceLastAutosave;

    private VirtualButton _escapeKey;

    // Map
    private TmxMap _tmxMap;
    private Vector2 _playerSpawnPosition;
    private Entity _tiledEntity;
    private TmxList<TmxObject> _dangerSpawns;
    private List<Entity> _dangerZones = new();
    private TmxList<TmxObject> _teleportTriggerObjects;

    // Player
    private Entity _playerEntity;
    private Player _playerComponent;

    // Camera
    private const float MoveSpeed = 2;
    private VirtualIntegerAxis _xAxisInput;
    private VirtualIntegerAxis _yAxisInput;
    private Vector2 _velocity;

    #region UI Variables

    private const int ScreenSpaceRenderLayer = 999;
    private UICanvas _uiCanvas;

    // In-Game
    private Table _inGameTable;
    private Label _scoreLabel;

    private Table _dataTable;
    private Label _itemScoreLabel;

    // Pause Menu
    private PauseMenu pauseMenu;

    #endregion

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GameScene"/> class.
    /// </summary>
    public GameScene() : base()
    {
        _gameData = new GameData();
        SetupUI();
    }

    /// <summary>
    /// Sets up the scene with the specified map path.
    /// </summary>
    /// <param name="mapPath"></param>
    public void SetupScene(string mapPath)
    {
        // Initialize the scene renderers
        InitializeScene();
        Debug.Log("Scene initialized");

        // Load the game data
        LoadGameData();
        Debug.Log("Game data loaded");

        // Load the map
        LoadMap(mapPath);
        Debug.Log($"Map loaded from path: {mapPath}");

        // Setup the camera
        SetupCamera();
        Debug.Log("Camera setup completed");

        // Setup the input
        SetupInput();
        Debug.Log("Input setup completed");

        // Setup the UI
        // NOTE: This should be the last method called in the setup to ensure all UI elements are rendered on top.
        SetupUI();
        Debug.Log("UI setup completed");
    }

    /// <summary>
    /// Sets the scene renderers.
    /// </summary>
    private void InitializeScene()
    {
        AddRenderer(new DefaultRenderer());
        AddRenderer(new ScreenSpaceRenderer(100, ScreenSpaceRenderLayer));
        AddRenderer(new RenderLayerExcludeRenderer(0, ScreenSpaceRenderLayer));
    }

    /// <summary>
    /// Loads the game data for the current save ID.
    /// </summary>
    private void LoadGameData()
    {
        // Initialize the save manager
        _saveManager = new SaveManager();

        // Attempt to get the current save ID
        if (_saveManager.GetCurrentID(out int saveId) && saveId > 0)
        {
            // Load the game data for the current save ID
            _saveManager.LoadGame(saveId, out _gameData);
            Debug.Log($"Game data loaded for save ID: {saveId}");
        }
        else
        {
            Debug.Log("No valid save ID found");
        }
    }

    /// <summary>
    /// Loads the Tiled map from the specified path.
    /// </summary>
    /// <param name="mapPath"></param>
    private void LoadMap(string mapPath)
    {
        // Load the Tiled map from the specified path
        _tmxMap = Content.LoadTiledMap(mapPath);
        Debug.Log($"Tiled map loaded from path: {mapPath}");

        // Setup the Tiled entity
        SetupTiledEntity();
        Debug.Log("Tiled entity setup completed");

        // Setup the player entity
        SetupPlayer();
        Debug.Log("Player setup completed");

        // Generate items on the map
        GenerateItems();
        Debug.Log("Items generated on the map");

        // Setup danger zones
        SetupDangerZones();
        Debug.Log("Danger zones setup completed");

        // Setup teleporters
        SetupTeleporters();
        Debug.Log("Teleporters setup completed");
    }

    /// <summary>
    /// Sets up the Tiled entity for the map.
    /// </summary>
    private void SetupTiledEntity()
    {
        // Destroy the existing Tiled entity if it exists
        _tiledEntity?.Destroy();

        // Create a new Tiled entity with the Tiled map renderer
        _tiledEntity = CreateEntity("tiled-map-entity");
        _tiledEntity.AddComponent(new TiledMapRenderer(_tmxMap, "Walls"));
    }

    /// <summary>
    /// Sets up the player entity for the map.
    /// </summary>
    private void SetupPlayer()
    {
        // Get the player spawn position from the Tiled map
        _playerSpawnPosition = GetPlayerSpawnPosition();

        // Destroy the existing player entity if it exists
        _playerEntity?.Destroy();

        // Create a new player entity with the player component
        _playerEntity = CreateEntity("player", _playerSpawnPosition);
        _playerComponent = _playerEntity.AddComponent(new Player(_playerSpawnPosition));

        // Add a BoxCollider and TiledMapMover to the player entity
        _playerEntity.AddComponent(new BoxCollider(-8, -16, 16, 32));
        _playerEntity.AddComponent(new TiledMapMover(_tmxMap.GetLayer<TmxLayer>("Walls")));

        // Set the player entity properties
        _playerEntity.Position = _playerSpawnPosition;
        _playerComponent.ItemScore = _gameData.ItemScore;
        _playerComponent.RespawnPosition = _playerSpawnPosition;
    }

    /// <summary>
    /// Gets the player spawn position from the Tiled map.
    /// </summary>
    /// <returns></returns>
    private Vector2 GetPlayerSpawnPosition()
    {
        // Get the player spawn point from the Tiled map
        var spawnPoint = _tmxMap.GetObjectGroup("SpawnPoints").Objects[_gameData.LastTeleporter];
        return new Vector2(spawnPoint.X, spawnPoint.Y);
    }

    /// <summary>
    /// Generates items on the map.
    /// </summary>
    private void GenerateItems()
    {
        // Return if the player is spawning at an item
        if (_gameData.LastTeleporter == "item")
            return;

        // Select a random item name from the options
        string[] itemNameOptions = { "5-score", "10-score", "double-score", "triple-score", "staircase" };
        var randomItemName = itemNameOptions[Nez.Random.NextInt(5)];

        // Place a staircase at its guarenteed position
        if (_gameData.Map[_gameData.Position[0], _gameData.Position[1]] == 4)
        {
            randomItemName = "staircase";
        }

        // Create a new entity for the item at the spawn point
        var upgradeSpawnPoint = _tmxMap.GetObjectGroup("SpawnPoints").Objects["item"];
        var newUpgrade = CreateEntity(randomItemName, new Vector2(upgradeSpawnPoint.X, upgradeSpawnPoint.Y));
        newUpgrade.SetTag(271);

        // Add a BoxCollider to the item entity
        var collider = newUpgrade.AddComponent(new BoxCollider(16, 48));
        collider.IsTrigger = true;

        // Load the texture for the item and create a sprite
        var texture = Content.Load<Texture2D>($"Misc/{randomItemName}");
        var sprite = new Sprite(texture, texture.Bounds);

        // Add the Upgrade and SpriteRenderer components to the item entity
        newUpgrade.AddComponent(new Upgrade());
        newUpgrade.AddComponent(new SpriteRenderer(sprite));

        Debug.Log($"Generated item: {randomItemName} at position: {upgradeSpawnPoint.X}, {upgradeSpawnPoint.Y}");
    }

    /// <summary>
    /// Sets up the danger zones on the map.
    /// </summary>
    private void SetupDangerZones()
    {
        // Get the list of danger zone objects from the Tiled map
        _dangerSpawns = _tmxMap.GetObjectGroup("DangerZones").Objects;

        // Iterate through each danger zone object and create an entity for it
        foreach (TmxObject danger in _dangerSpawns)
        {
            // Create a new entity for the danger zone
            var zone = CreateEntity(danger.Name, new Vector2(danger.X, danger.Y));
            zone.SetTag(4);

            // Add a polygon collider to the danger zone entity
            var collider = zone.AddComponent(new PolygonCollider(danger.Points));
            collider.IsTrigger = true;

            // Add a DangerZone component to the danger zone entity
            zone.AddComponent(new DangerZone());

            Debug.Log($"Generated danger zone at position: {danger.X}, {danger.Y}");
        }
    }

    /// <summary>
    /// Sets up the teleporters on the map.
    /// </summary>
    private void SetupTeleporters()
    {
        // Get the list of teleporter objects from the Tiled map
        var tmxTeleporters = _tmxMap.GetObjectGroup("Teleporters").Objects;
        foreach (TmxObject teleporter in tmxTeleporters)
        {
            // Create a new entity for the teleporter at the specified position
            var point = CreateEntity(teleporter.Name, new Vector2(teleporter.X + (teleporter.Width / 2), teleporter.Y + (teleporter.Height / 2)));

            // Add a BoxCollider and Teleporter component to the teleporter entity
            var collider = point.AddComponent(new BoxCollider(teleporter.Width, teleporter.Height));
            collider.IsTrigger = true;
            point.AddComponent(new Teleporter());
            point.SetTag(42);

            Debug.Log($"Teleporter created: {teleporter.Name} at position: {teleporter.X}, {teleporter.Y}");
        }
    }

    /// <summary>
    /// Sets up the UI for the game scene.
    /// </summary>
    private void SetupUI()
    {
        // Create and configure the UI canvas
        _uiCanvas = CreateEntity("uiCanvas").AddComponent(new UICanvas());
        _uiCanvas.IsFullScreen = false;
        _uiCanvas.RenderLayer = ScreenSpaceRenderLayer;

        // Setup the in-game UI elements
        SetupInGameUI();
        SetupPauseWindow();
        SetupDataTable();

        Debug.Log("UI setup");
    }

    /// <summary>
    /// Sets up the in-game UI elements.
    /// </summary>
    private void SetupInGameUI()
    {
        // Create a new table for the in-game UI elements
        _inGameTable = _uiCanvas.Stage.AddElement(new Table());
        _inGameTable.SetFillParent(true).Top().Left().Pad(10);
    }

    /// <summary>
    /// Sets up the pause window for the game scene.
    /// </summary>
    private void SetupPauseWindow()
    {
        // Create a new pause menu for the game scene
        pauseMenu = new PauseMenu(_uiCanvas, _isPaused, _gameData);
        pauseMenu.Hide();
    }

    /// <summary>
    /// Sets up the data table for the game scene.
    /// </summary>
    private void SetupDataTable()
    {
        // Create a new table for the data elements
        _dataTable = _uiCanvas.Stage.AddElement(new Table());
        _dataTable.Top().Right().Pad(10).SetFillParent(true);
        _itemScoreLabel = _dataTable.Add(new Label($"Score: {_gameData.ItemScore}")).GetElement<Label>();
        _itemScoreLabel.SetColor(Color.White);
        _dataTable.SetVisible(true);
    }

    /// <summary>
    /// Hides all UI elements.
    /// </summary>
    private void HideAllUI()
    {
        pauseMenu.Hide();
    }

    /// <summary>
    /// Sets up the camera for the game scene.
    /// </summary>
    private void SetupCamera()
    {
        // Create a new follow camera for the player entity
        var followCam = new FollowCamera(_playerEntity, FollowCamera.CameraStyle.CameraWindow, FollowCamera.Measurement.ScaledCameraBounds)
        {
            FollowLerp = 0.1f
        };

        // Add the follow camera to the player entity
        Camera.Entity.AddComponent(followCam);
        Camera.SetZoom(0.75f);
    }

    /// <summary>
    /// Sets up the input for the camera.
    /// </summary>
    private void SetupInput()
    {
        // Create the input axes for the camera
        _xAxisInput = new VirtualIntegerAxis();
        _xAxisInput.Nodes.Add(new VirtualAxis.GamePadRightStickX());
        _xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

        _yAxisInput = new VirtualIntegerAxis();
        _yAxisInput.Nodes.Add(new VirtualAxis.GamePadRightStickY());
        _yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Down, Keys.Up));

        _escapeKey = new VirtualButton();
        _escapeKey.Nodes.Add(new VirtualButton.GamePadButton(0, Buttons.Start));
        _escapeKey.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Escape));
    }

    /// <summary>
    /// Updates the game scene.
    /// </summary>
    public override void Update()
    {
        // Call the base update method
        base.Update();

        // Update the time played
        _gameData.TimePlayed += Time.DeltaTime;
        Debug.Log($"Time played: {_gameData.TimePlayed}");

        // Upate the item score label
        _itemScoreLabel.SetText(_gameData.ItemScore.ToString());

        // Check if the player has teleported
        if (_playerComponent.Teleport != "item")
        {
            Debug.Log("Player teleported");
            HandleTeleport();
            return;
        }

        // Update the game data item score if it has changed
        if (_playerComponent.ItemScore != _gameData.ItemScore)
        {
            Debug.Log("Item score updated");
            _gameData.ItemScore = _playerComponent.ItemScore;
        }

        // Check if the player has reached a staircase
        if (_playerComponent.Staircase)
        {
            Debug.Log("Player reached staircase");
            HandleStaircase();
            return;
        }

        // Handle the autosave and pause
        HandleAutosave();
        HandlePause();
        _playerComponent.Update(_isPaused);

        // Update the camera position
        UpdateCameraPosition();
    }

    /// <summary>
    /// Handles the teleportation of the player.
    /// </summary>
    private void HandleTeleport()
    {
        // Update the player position
        UpdatePlayerPosition();

        // Load the next level
        _playerComponent.Teleport = "item";
        _gameData.RoomsVisited++;
        SaveGame();
        var scene = new Level1(_gameData.GetMapAtPosition(_gameData.Position[0], _gameData.Position[1]));
        scene.ClearColor = new Color(14, 7, 27, 255);
        Core.StartSceneTransition(new FadeTransition(() => scene));
    }

    /// <summary>
    /// Updates the player position after teleportation.
    /// </summary>
    private void UpdatePlayerPosition()
    {
        // Update the player position based on the teleport direction
        switch (_playerComponent.Teleport.ToLower())
        {
            case "north":
                _gameData.LastTeleporter = "south";
                _gameData.Position[1]--;
                break;
            case "east":
                _gameData.LastTeleporter = "west";
                _gameData.Position[0]++;
                break;
            case "south":
                _gameData.LastTeleporter = "north";
                _gameData.Position[1]++;
                break;
            case "west":
                _gameData.LastTeleporter = "east";
                _gameData.Position[0]--;
                break;
        }
    }

    /// <summary>
    /// Handles the staircase event.
    /// </summary>
    private void HandleStaircase()
    {
        _playerComponent.Staircase = false;
        _gameData.RoomsVisited++;
        // Check if the player has reached the final floor
        if (_gameData.Floor >= 2)
        {
            SaveGame();
            Core.StartSceneTransition(new FadeTransition(() => new WinGame()));
        }
        else
        {
            // Move the player to the next floor
            MoveToNextFloor();
        }
    }

    /// <summary>
    /// Moves the player to the next floor.
    /// </summary>
    private void MoveToNextFloor()
    {
        // Update the game data for the next floor
        _gameData.Floor++;
        _gameData.LastTeleporter = "item";

        // Generate a new map for the next floor
        var mapGenerator = new MapGenerator(10, 10);
        _gameData.Map = mapGenerator.GenerateMap(10);
        string mapStr = "";
        for (int y = 0; y < _gameData.Map.GetLength(0); y++)
        {
            for (int x = 0; x < _gameData.Map.GetLength(1); x++)
            {
                if (_gameData.Map[x, y] == 3)
                {
                    mapStr = _gameData.GetMapAtPosition(x, y);
                    _gameData.Position = new int[] { x, y };
                    break;
                }
            }
        }

        // Save the game data and start the next level
        SaveGame();
        var scene = new Level1(mapStr);
        scene.ClearColor = new Color(14, 7, 27, 255);
        Core.StartSceneTransition(new FadeTransition(() => scene));
    }

    /// <summary>
    /// Handles the autosave event.
    /// </summary>
    private void HandleAutosave()
    {
        _timeSinceLastAutosave += Time.DeltaTime;

        // Save the game every 10 minutes
        if (_timeSinceLastAutosave >= 10 * 60)
            SaveGame();
    }

    /// <summary>
    /// Handles the pause event.
    /// </summary>
    private void HandlePause()
    {
        if (pauseMenu == null)
            return;

        // Check if the escape key is pressed
        if (_escapeKey.IsReleased)
        {
            // Toggle the pause menu visibility
            if (pauseMenu.IsVisible)
            {
                HideAllUI();
                _isPaused = false;
            }
            else
            {
                pauseMenu.Display();
            }
        }

        // Update the pause state
        if (pauseMenu.IsVisible)
            _isPaused = true;
        else
            _isPaused = false;
    }

    /// <summary>
    /// Updates the camera position based on the input.
    /// </summary>
    private void UpdateCameraPosition()
    {
        // Get the move direction and velocity
        var moveDir = new Vector2(_xAxisInput.Value, _yAxisInput.Value);
        _velocity.X = moveDir.X < 0 ? -MoveSpeed : moveDir.X > 0 ? MoveSpeed : 0;
        _velocity.Y = moveDir.Y < 0 ? MoveSpeed : moveDir.Y > 0 ? -MoveSpeed : 0;

        // Update the camera position if the game is not paused
        if (!_isPaused)
            Camera.Entity.Position += _velocity;
    }

    /// <summary>
    /// Saves the game data.
    /// </summary>
    public void SaveGame()
    {
        _gameData.ItemScore = _playerComponent.ItemScore;
        _saveManager.SaveGame(_gameData);
        _saveManager.SetCurrentID(_gameData.SaveID);
    }

    /// <summary>
    /// Initializes the game scene.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        ClearColor = new Color(255, 0, 0, 255);
    }

    // Not used, but required by the Scene class
    public override void Unload() { }
}