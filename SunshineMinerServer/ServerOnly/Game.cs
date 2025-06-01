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

    private Dictionary<string, Manager> managers = new Dictionary<string, Manager>();
    //public Gate gate { get; private set; } // handle connection and msg
    //public EntityManager entityManager { get; private set; } // manage entities
    //public EventManager eventManager { get; private set; } // manage events and global events
    //public AccountManager accountManager { get; private set; } // manage account and online states
    //public TimerManager timerManager { get; private set; }

    public Game()
    {
        Instance = this;
        CreateManager<Gate>("Gate");
        CreateManager<EntityManager>("EntityManager");
        CreateManager<EventManager>("EventManager");
        CreateManager<TimerManager>("TimerManager");
        CreateManager<AccountManager>("AccountManager");
    }

    /*
     * Start game (in main thread)
     */
    public void Start()
    {
        StartManagers();
        isRunning = true;
        Debugger.Log("Server game starts...");

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
                UpdateManagers();
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
        StopManagers();
        Debugger.Log("Server game ends...");
    }

    #region REGION_MANAGER

    private void CreateManager<T>(string name) where T : Manager, new()
    {
        T mgr = new T();
        managers[name] = mgr;
    }

    private T? GetManager<T>(string name) where T : Manager
    {
        if (managers.TryGetValue(name, out Manager manager))
        {
            return (T)manager;
        }
        return null;
    }

    private void StartManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Start();
        }
    }

    private void UpdateManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Update();
        }
    }

    private void StopManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Stop();
        }
        managers.Clear();
    }

    public Gate? gate
    {
        get
        {
            return GetManager<Gate>("Gate");
        }
    }

    public EntityManager? entityManager
    {
        get
        {
            return GetManager<EntityManager>("EntityManager");
        }
    }

    public EventManager? eventManager
    {
        get
        {
            return GetManager<EventManager>("EventManager");
        }
    }

    public TimerManager? timerManager
    {
        get
        {
            return GetManager<TimerManager>("TimerManager");
        }
    }

    public AccountManager? accountManager
    {
        get
        {
            return GetManager<AccountManager>("AccountManager");
        }
    }

    #endregion

    #region REGION_RPC

    public void InvokeRpc(Msg msg, Proxy proxy)
    {
        string tgtId = msg.tgtId;
        object instance = null;
        if (managers.ContainsKey(tgtId))
        {
            instance = managers[tgtId];
        }
        else
        {
            instance = entityManager.GetEntity(tgtId);
        }
        if (instance == null)
        {
            return;
        }

        var method = instance.GetType().GetMethod(msg.methodName);
        if (method == null)
        {
            return;
        }

        var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
        if (rpcAttr == null)
        {
            return;
        }

        CustomList args = msg.arg.WrapInList();

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
