using Nez.Persistence.Binary;
using System;
using Nez;
using System.IO;
using PlatformerGame.Scenes;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PlatformerGame.Helpers
{
    /// <summary>
    /// Game data class for storing game data.
    /// </summary>
    public class GameData : IPersistable
    {
        public int SaveID;              // ID of the savegame
        public string Name;             // Character name
        public int Floor;               // Current floor
        public int[] Position;          // Room position on map
        public string LastTeleporter;   // Last teleporter used (teleported from)
        public string LastSaved;        // Last saved time
        public int Score;               // Total score
        public int ItemScore;           // Score from items
        public float TimePlayed;        // Total time played in seconds
        public int RoomsVisited;        // Total rooms visited
        public int[,] Map;              // 2 dimensional map array (for use in game)
        public int[] Map1D;             // 1 dimensional map array (for storing data)

        /// <summary>
        /// Persists the game data to a save file.
        /// </summary>
        /// <param name="writer"></param>
        public void Persist(IPersistableWriter writer)
        {
            writer.Write(SaveID);
            writer.Write(Name);
            writer.Write(Floor);
            writer.Write(Position);
            writer.Write(LastTeleporter);
            writer.Write(DateTime.Now.ToString());
            writer.Write(Score);
            writer.Write(ItemScore);
            writer.Write(TimePlayed);
            writer.Write(RoomsVisited);
            ConvertMapTo1D();
            writer.Write(Map1D);
        }

        /// <summary>
        /// Recovers the game data from a save file.
        /// </summary>
        /// <param name="reader"></param>
        public void Recover(IPersistableReader reader)
        {
            SaveID = reader.ReadInt();
            Name = reader.ReadString();
            Floor = reader.ReadInt();
            Position = reader.ReadIntArray();
            LastTeleporter = reader.ReadString();
            LastSaved = reader.ReadString();
            Score = reader.ReadInt();
            ItemScore = reader.ReadInt();
            TimePlayed = reader.ReadFloat();
            RoomsVisited = reader.ReadInt();
            Map1D = reader.ReadIntArray();
            ConvertMap1Dto2D();
        }

        /// <summary>
        /// Creates a new game data object.
        /// </summary>
        /// <param name="saveID"></param>
        /// <param name="name"></param>
        /// <param name="floor"></param>
        /// <param name="position"></param>
        /// <param name="lastTeleporter"></param>
        /// <param name="lastSaved"></param>
        /// <param name="score"></param>
        /// <param name="itemScore"></param>
        /// <param name="timePlayed"></param>
        /// <param name="roomsVisited"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static GameData New(int saveID, string name, int floor, int[] position, string lastTeleporter, string lastSaved, int score, int itemScore, float timePlayed, int roomsVisited, int[,] map)
        {
            var data = new GameData
            {
                SaveID = saveID,
                Name = name,
                Floor = floor,
                Position = position,
                LastTeleporter = lastTeleporter,
                LastSaved = lastSaved,
                Score = score,
                ItemScore = itemScore,
                TimePlayed = timePlayed,
                RoomsVisited = roomsVisited,
                Map = map
            };
            data.ConvertMapTo1D();
            return data;
        }

        /// <summary>
        /// Converts the 2D map array to a 1D array.
        /// </summary>
        public void ConvertMapTo1D()
        {
            Map1D = new int[Map.GetLength(0) * Map.GetLength(1)];
            for (int y = 0; y < Map.GetLength(0); y++)
            {
                for (int x = 0; x < Map.GetLength(1); x++)
                {
                    Map1D[x * Map.GetLength(1) + y] = Map[x, y];
                }
            }
        }

        /// <summary>
        /// Converts the 1D map array to a 2D array.
        /// </summary>
        public void ConvertMap1Dto2D()
        {
            Map = new int[(int)Math.Sqrt(Map1D.Length), (int)Math.Sqrt(Map1D.Length)];
            for (int y = 0; y < Map.GetLength(0); y++)
            {
                for (int x = 0; x < Map.GetLength(1); x++)
                {
                    Map[x, y] = Map1D[x * Map.GetLength(1) + y];
                }
            }
        }

        /// <summary>
        /// Gets the map at a specified position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public string GetMapAtPosition(int x, int y)
        {
            // NORTH, EAST, SOUTH, WEST
            bool[] neighbors = { false, false, false, false };
            if (y > 0 && Map[x, y - 1] > 0) neighbors[0] = true;
            if (x < (Map.GetLength(0) - 1) && Map[x + 1, y] > 0) neighbors[1] = true;
            if (y < (Map.GetLength(0) - 1) && Map[x, y + 1] > 0) neighbors[2] = true;
            if (x > 0 && Map[x - 1, y] > 0) neighbors[3] = true;

            if (neighbors[0] && neighbors[1] && neighbors[2] && neighbors[3]) return "cross-section-1";

            if (neighbors[1] && neighbors[2] && neighbors[3]) return "t-junction-north";
            if (neighbors[0] && neighbors[2] && neighbors[3]) return "t-junction-east";
            if (neighbors[0] && neighbors[1] && neighbors[3]) return "t-junction-south";
            if (neighbors[0] && neighbors[1] && neighbors[2]) return "t-junction-west";

            if (neighbors[1] && neighbors[2]) return "corner-tl";
            if (neighbors[2] && neighbors[3]) return "corner-tr";
            if (neighbors[0] && neighbors[1]) return "corner-bl";
            if (neighbors[0] && neighbors[3]) return "corner-br";

            if (neighbors[0] && neighbors[2]) return "vertical";
            if (neighbors[1] && neighbors[3]) return "horizontal";

            if (neighbors[0]) return "cap-north";
            if (neighbors[1]) return "cap-east";
            if (neighbors[2]) return "cap-south";
            if (neighbors[3]) return "cap-west";

            return "map";
        }
    }

    /// <summary>
    /// Save manager class for managing savegames.
    /// </summary>
    internal class SaveManager
    {
        // Constants
        private const string DataRoot = "PlatformerGame_DATA";
        private const string SavePath = $"{DataRoot}/Saves";
        private const string CurrentFile = "saveinfo.sa";

        private FileDataStore _saveDataStore;

        /// <summary>
        /// Constructor for the save manager.
        /// </summary>
        public SaveManager()
        {
            _saveDataStore = new FileDataStore(SavePath, FileDataStore.FileFormat.Binary);
        }

        /// <summary>
        /// Creates a new savegame.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool NewSavegame(string name)
        {
            // Generate a unique save ID based on the current Unix time.
            int saveID = Convert.ToInt32(DateTimeOffset.Now.ToUnixTimeSeconds());

            // Create a new game data object.
            var data = GameData.New(saveID, name, 0, new int[] { -1, -1 }, "item", DateTime.Now.ToString(), 0, 0, 0.0f, 0, new int[10, 10]);

            // Generate a new map for the game.
            MapGenerator mapGenerator = new MapGenerator(10, 10);
            data.Map = mapGenerator.GenerateMap(10);

            // Find the starting position in the map (where the value is 3).
            for (int y = 0; y < data.Map.GetLength(0); y++)
            {
                for (int x = 0; x < data.Map.GetLength(1); x++)
                {
                    if (data.Map[x, y] == 3)
                    {
                        data.Position = new int[] { x, y };
                        break;
                    }
                }
            }

            // Save the game data to the file system.
            if (SaveGame(data))
            {
                if (SetCurrentID(data.SaveID))
                {
                    Debug.Log($"New savegame created with ID: {data.SaveID}");
                    return true;
                }
                else
                {
                    Debug.Log("Failed to set current save ID.");
                }
            }
            else
            {
                Console.WriteLine("Failed to save game data.");
            }

            return false;
        }

        /// <summary>
        /// Sets the current save ID.
        /// </summary>
        /// <param name="saveID"></param>
        /// <returns></returns>
        public bool SetCurrentID(int saveID)
        {
            // Check if the save file exists.
            if (File.Exists($"{SavePath}/{saveID}"))
            {
                // Write the save ID to the current file.
                try
                {
                    File.WriteAllText($"{DataRoot}/{CurrentFile}", Convert.ToString(saveID));
                    return true;
                }
                catch (Exception) { }
            }
            return false;
        }

        /// <summary>
        /// Gets the current save ID.
        /// </summary>
        /// <param name="saveID"></param>
        /// <returns></returns>
        public bool GetCurrentID(out int saveID)
        {
            // Attempt to read the current save ID from the file.
            try
            {
                saveID = Convert.ToInt32(File.ReadAllText($"{DataRoot}/{CurrentFile}"));
                if (File.Exists($"{SavePath}/{saveID}"))
                    return true;
                else
                    return false;
            }
            catch (Exception) { saveID = default; }
            return false;
        }

        /// <summary>
        /// Saves the game data to the file system.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SaveGame(GameData data)
        {
            // Attempt to save the game data to the file system.
            try
            {
                _saveDataStore.Save($"{data.SaveID}", data);
                return true;
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Loads a game from the file system.
        /// </summary>
        /// <param name="saveID"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool LoadGame(int saveID, out GameData data)
        {
            // Create a new game data object.
            data = new GameData();

            // Attempt to load the game data from the file system.
            try
            {
                _saveDataStore.Load($"{saveID}", data);
                return true;
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Deletes a game from the file system.
        /// </summary>
        /// <param name="saveID"></param>
        /// <returns></returns>
        public bool DeleteGame(int saveID)
        {
            // Attempt to delete the game data from the file system.
            try
            {
                File.Delete($"{SavePath}/{saveID}");
                return true;
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// Lists all savegames in the file system.
        /// </summary>
        /// <returns></returns>
        public List<GameData> ListSaves()
        {
            // Create a new list of game data objects.
            List<GameData> list = new List<GameData>();

            // Get all files in the save directory.
            string[] filesStrArr = Directory.GetFiles(SavePath);
            // Iterate through each file and attempt to load the game data.
            foreach (string fileStr in filesStrArr)
            {
                // Attempt to load the game data from the file.
                try
                {
                    if (LoadGame(Convert.ToInt32(Path.GetFileNameWithoutExtension(fileStr)), out GameData fileData))
                    {
                        list.Add(fileData);
                    }
                }
                catch (Exception) { }
            }

            return list;
        }

        /// <summary>
        /// Starts the game from the current save ID.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StartGame()
        {
            // Get the current save ID.
            if (GetCurrentID(out int saveID))
            {
                // Load the game data from the file system.
                LoadGame(saveID, out var gameData);
                var mapStr = gameData.GetMapAtPosition(gameData.Position[0], gameData.Position[1]);

                // Check if the map string is null.
                if (mapStr == null)
                {
                    throw new Exception("mapStr is null");
                }

                // Create a new level scene with the map string.
                var scene = new Level1(mapStr)
                {
                    levelName = mapStr,
                    ClearColor = new Color(14, 7, 27, 255)
                };

                // Start the scene transition.
                SaveGame(gameData);
                Core.StartSceneTransition(new FadeTransition(() => scene));
            }
        }
    }
}
