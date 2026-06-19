namespace TestApp;

class Program
{
    static void Main(string[] args)
    {
        for (int i = 0; i < 20; i++)
        {
            MapGenerator generator = new MapGenerator(10, 10);
            generator.GenerateMap(20);
            generator.PrintMap();
            Console.ReadLine();
        }
    }
}