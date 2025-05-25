// Seeusing System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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
