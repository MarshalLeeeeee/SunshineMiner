using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Game
{
    private Gate gate; // handle connection and msg
    private int tickInterval; // update interval

    public Game(int port)
    {
        gate = new Gate(port);
        tickInterval = 10; // 10ms, 0.01s
    }

    public void Start()
    {
        gate.Start();
        Console.WriteLine("Server game starts...");

        long nextTickTime = 0;
        while (true)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (currentTime >= nextTickTime)
            {
                float dt = (float)(currentTime - (nextTickTime - tickInterval)) / 1000f;
                gate.Update(dt);
                nextTickTime = currentTime + tickInterval;
            }

            Thread.Sleep(1);
        }
    }
}
