using PlatformerGame.Helpers;
using System.Text;

namespace mparse
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(args.Length);

            Console.WriteLine($"Reading save file: {args[0]}");

            if (!File.Exists($"{SaveManager.SavePath}\\{args[0]}"))
            {
                Console.WriteLine($"File not found: {args[0]}");
                return;
            }

            SaveManager saveManager = new SaveManager();
            GameData gameData;

            if (int.TryParse(args[0], out int saveID))
            {
                if (saveManager.LoadGame(saveID, out gameData))
                {
                    Console.WriteLine($"Save ID: {gameData.SaveID}");
                    Console.WriteLine($"Name: {gameData.Name}");
                    Console.WriteLine($"Floor: {gameData.Floor}");
                    Console.WriteLine($"Position: {gameData.Position}");
                    Console.WriteLine($"Last teleporter: {gameData.LastTeleporter}");
                    Console.WriteLine($"Last saved: {gameData.LastSaved}");
                    Console.WriteLine($"Score: {gameData.Score}");
                    Console.WriteLine($"Item score: {gameData.ItemScore}");
                    Console.WriteLine($"Time played: {gameData.TimePlayed}");
                    Console.WriteLine($"Rooms visited: {gameData.RoomsVisited}");
                    Console.WriteLine($"Map: {gameData.Map}");
                    Console.WriteLine($"Map1D: {gameData.Map1D}");

                    StringBuilder sb = new StringBuilder();
                    for (int y = 0; y < 10; y++)
                    {
                        for (int x = 0; x < 10; x++)
                        {
                            if (gameData.Map[x, y] <= 0)
                                sb.Append('.');
                            else if (gameData.Map[x, y] == 2)
                                sb.Append('@');
                            else if (gameData.Map[x, y] == 3)
                                sb.Append('!');
                            else
                                sb.Append('#');
                            sb.Append(' ');
                        }
                        sb.AppendLine();
                    }
                    Console.WriteLine(sb.ToString());
    }
                else
                {
                    Console.WriteLine("Failed to load save ID: {args[0]}");
                }
            }
            else
            {
                Console.WriteLine("Invalid save ID: {args[0]}");
            }
        }
    }
}
