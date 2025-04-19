using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Game
{
    private Gate gate;

    public Game(int port)
    {
        gate = new Gate(port);
    }

    public void Start()
    {
        gate.Start();
        Console.WriteLine("Server game starts...");
    }
}
