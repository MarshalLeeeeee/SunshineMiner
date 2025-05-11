using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

internal class Game : IDisposable
{
    // Singleton instance
    public static Game Instance { get; private set; } = null!;

    private bool isRunning;
    public float dt { get; private set; } // current delta time in tick
    public Gate gate { get; private set; } // handle connection and msg
    public EntityManager entityManager { get; private set; } // manage entities
    public EventManager eventManager { get; private set; } // manage events and global events
    public AccountManager accountManager { get; private set; } // manage account and online states
    public TimerManager timerManager { get; private set; }

    public Game()
    {
        Instance = this;
        gate = new Gate();
        entityManager = new EntityManager();
        eventManager = new EventManager();
        accountManager = new AccountManager();
        timerManager = new TimerManager();
    }

    /*
     * Start game (in main thread)
     */
    public void Start()
    {
        gate.Start();
        entityManager.Start();
        eventManager.Start();
        timerManager.Start();
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
                dt = (float)(currentTime - (nextTickTime - Const.TickInterval)) / 1000f;
                gate.Update();
                entityManager.Update();
                eventManager.Update();
                timerManager.Update();

                nextTickTime = currentTime + Const.TickInterval;
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
        timerManager.Stop();
        Console.WriteLine("Server game ends...");
    }

    #region REGION_RPC

    public void InvokeRpc(object instance, Proxy proxy, string callerId, string methodName, CustomType argsRaw)
    {
        var method = instance.GetType().GetMethod(methodName);
        if (method == null)
        {
            return;
        }

        var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
        if (rpcAttr == null)
        {
            return;
        }

        CustomList args = (CustomList)argsRaw;

        int rpcType = rpcAttr.rpcType;
        int[] rpcArgs = rpcAttr.argTypes;

        int argsCount = args.Count;
        int rpcArgsCount = rpcArgs.Length;
        if (argsCount != rpcArgsCount)
        {
            return;
        }

        int i = 0;
        while (i < rpcArgsCount)
        {
            CustomType arg = args[i];
            if (arg.type != rpcArgs[i])
            {
                return;
            }
            i += 1;
        }

        List<object> methodArgs = new List<object>();
        foreach (CustomType arg in args)
        {
            methodArgs.Add(arg);
        }
        methodArgs.Add(proxy);
        method.Invoke(instance, methodArgs.ToArray());
    }

    #endregion
}
