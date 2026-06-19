using Nez.Persistence.Binary;
using System;
using Nez;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PlatformerGame.Helpers
{
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

        public void Persist(IPersistableWriter writer)
        {
        }

        public void Recover(IPersistableReader reader)
        {
            // Read all data from the save file.
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

        public void ConvertMap1Dto2D()
        {
            // Convert the 1D map array to a 2D array for use in game.
            Map = new int[(int)Math.Sqrt(Map1D.Length), (int)Math.Sqrt(Map1D.Length)];
            for (int y = 0; y < Map.GetLength(0); y++)
            {
                for (int x = 0; x < Map.GetLength(1); x++)
                {
                    Map[x, y] = Map1D[x * Map.GetLength(1) + y];
                }
            }
        }

        public string GetMapAtPosition(int x, int y)
        {
            // Get the map tile at a specific position.
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

    struct SettingsData
    {
        // Add per setting added.
    }

    internal class SaveManager
    {
        public const string DataRoot = "PlatformerGame_DATA";
        public const string SavePath = $"{DataRoot}/Saves";

        private FileDataStore _saveDataStore;

        public SaveManager()
        {
            _saveDataStore = new FileDataStore(SavePath, FileDataStore.FileFormat.Binary);
        }

        public bool LoadGame(int saveID, out GameData data)
        {
            data = new GameData();
            try
            {
                _saveDataStore.Load($"{saveID}", data);
                return true;
            }
            catch (Exception) { }
            return false;
        }

        public List<GameData> ListSaves()
        {
            List<GameData> list = new List<GameData>();

            string[] filesStrArr = Directory.GetFiles(SavePath);
            foreach (string fileStr in filesStrArr)
            {
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
    }
}
