
class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game())
        {
            game.Start();
        }
        Debugger.Log("Press enter to exit thread...");
        Console.ReadLine();
    }
}
