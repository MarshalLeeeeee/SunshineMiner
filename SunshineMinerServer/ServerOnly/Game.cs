using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Game : IDisposable
{
    // Singleton instance
    public static Game? Instance { get; private set; }

    private bool isRunning;
    public float dt { get; private set; } // current delta time in tick
    public Gate gate { get; private set; } // handle connection and msg
    public EntityManager entityManager { get; private set; } // manage entities
    public EventManager eventManager { get; private set; } // manage events and global events

    public Game()
    {
        Instance = this;
        gate = new Gate();
        entityManager = new EntityManager();
        eventManager = new EventManager();
    }

    /*
     * Start game (in main thread)
     */
    public void Start()
    {
        gate.Start();
        entityManager.Start();
        eventManager.Start();
        isRunning = true;
        Console.WriteLine("Server game starts...");

        // ctrl c in termin to stop game
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            isRunning = false;
        };

        // controlled tick
        long nextTickTime = 0;
        while (isRunning)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (currentTime >= nextTickTime)
            {
                dt = (float)(currentTime - (nextTickTime - ServerConst.TickInterval)) / 1000f;
                gate.Update();
                entityManager.Update();
                eventManager.Update();

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
        entityManager.Stop();
        eventManager.Stop();
        Console.WriteLine("Server game ends...");
    }
}
