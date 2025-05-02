using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Game : IDisposable
{
    private Gate gate; // handle connection and msg
    private bool isRunning;

    public Game()
    {
        gate = new Gate();
    }

    /*
     * Start game (in main thread)
     */
    public void Start()
    {
        gate.Start();
        Console.WriteLine("Server game starts...");
        isRunning = true;

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            isRunning = false;
        };

        long nextTickTime = 0;
        while (isRunning)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (currentTime >= nextTickTime)
            {
                float dt = (float)(currentTime - (nextTickTime - ServerConst.TickInterval)) / 1000f;
                gate.Update(dt);
                nextTickTime = currentTime + ServerConst.TickInterval;
            }

            Thread.Sleep(1);
        }
    }

    /*
     * Recycle resources (in main thread)
     */
    public void Dispose()
    {
        isRunning = false;
        gate.Stop();
        Console.WriteLine("Server game ends...");
    }
}
