using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformerGame.Helpers
{
    /// <summary>
    /// Generates a map for the game.
    /// </summary>
    public class MapGenerator
    {
        // Map variables
        private int width;
        private int height;
        private int[,] map;
        private Random random;

        /// <summary>
        /// Constructor for the map generator.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public MapGenerator(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.map = new int[width, height];
            this.random = new Random();
        }

        /// <summary>
        /// Generates a map with a given amount of particles.
        /// </summary>
        /// <param name="particleCount"></param>
        /// <returns></returns>
        public int[,] GenerateMap(int particleCount)
        {
            // List of points to generate particles from
            List<(int, int)> points = new List<(int, int)>();

            // Initialize seed point
            {
                int x = random.Next(width);
                int y = random.Next(height);
                points.Add((x, y));
                map[x, y] = 1;
            }

            // Generate particles
            for (int i = 0; i < particleCount; i++)
            {
                // Random point
                (int x, int y) = points[random.Next(points.Count)];

                // Random travel distance and direction
                int travelDistance = random.Next(1, 3);
                int direction = random.Next(4);

                // Travel
                for (int j = 0; j < travelDistance; j++)
                {
                    if (x == 0 | x == this.width - 1 | y == 0 | y == this.height - 1) break;
                    
                    switch (direction)
                    {
                        case 1:
                            // Up
                            y--;
                            break;
                        case 2:
                            // Right
                            x++;
                            break;
                        case 3:
                            // Down
                            y++;
                            break;
                        case 4:
                            // Left
                            x--;
                            break;
                    }

                    // Check if point is valid
                    if (IsNeighborSeed(x, y))
                    {
                        points.Add((x, y));
                        map[x, y] = 1;
                        //break;
                    }
                }
            }

            // Regenerate if not enough particles
            if (points.Count < 10)
            {
                points.Clear();
                map = new int[width, height];
                map = GenerateMap(particleCount);
            }
            // Generate areas
            else GenerateAreas();

            // Return map
            return map;
        }

        /// <summary>
        /// Checks if a point is a neighbor seed.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsNeighborSeed(int x, int y)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    int nx = (x + dx + width) % width;
                    int ny = (y + dy + height) % height;
                    if (map[nx, ny] > 0) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Generates areas for the map.
        /// </summary>
        private void GenerateAreas()
        {
            // List of points to generate areas from
            List<(int, int)> points = new List<(int, int)>();
            
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    if (map[x, y] > 0)
                    {
                        points.Add((x, y));
                    }
                }
            }

            // Generate spawn
            {
                var randomPoint = random.Next(points.Count);
                (int x, int y) = points[randomPoint];
                points.RemoveAt(randomPoint);
                map[x, y] = 3;
            }
            
            // Generate staircase
            {
                var randomPoint = random.Next(points.Count);
                (int x, int y) = points[randomPoint];
                points.RemoveAt(randomPoint);
                map[x, y] = 4;
            }
        }

        /// <summary>
        /// Prints the map to the console.
        /// </summary>
        public void PrintMap()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[x, y] <= 0)
                        sb.Append('.');
                    else if (map[x, y] == 2)
                        sb.Append('@');
                    else if (map[x, y] == 3)
                        sb.Append('!');
                    else
                        sb.Append('#');
                    sb.Append(' ');
                }
                sb.AppendLine();
            }
            Console.WriteLine(sb.ToString());
        }
    }
}