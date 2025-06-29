using System.Reflection;

public class Game : GameCommon, IDisposable
{
    // Singleton instance
    public static Game Instance { get; private set; } = null!;

    private bool isRunning;
    public float dt { get; private set; } // current delta time in tick

    public Game()
    {
        Instance = this;
        InitManagers();
    }

    /*
     * Start game (in main thread)
     */
    public void Start()
    {
        EnableManagers();
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
            else
            {
                Thread.Sleep((int)(nextTickTime - currentTime));
            }
        }
    }

    /*
     * Recycle resources (in main thread)
     */
    public void Dispose()
    {
        isRunning = false;
        DisableManagers();
        Debugger.Log("Server game ends...");
    }

    #region REGION_MANAGER

    protected override void InitManagers()
    {
        CreateManager<Gate>();
        CreateManager<EntityManager>();
        CreateManager<EventManager>();
        CreateManager<TimerManager>();
        CreateManager<AccountManager>();
    }

    public Gate? gate
    {
        get
        {
            return GetManager<Gate>();
        }
    }

    public EntityManager? entityManager
    {
        get
        {
            return GetManager<EntityManager>();
        }
    }

    public EventManager? eventManager
    {
        get
        {
            return GetManager<EventManager>();
        }
    }

    public TimerManager? timerManager
    {
        get
        {
            return GetManager<TimerManager>();
        }
    }

    public AccountManager? accountManager
    {
        get
        {
            return GetManager<AccountManager>();
        }
    }

    #endregion

    #region REGION_RPC

    public void InvokeRpc(Msg msg, Proxy proxy)
    {
        string tgtId = msg.tgtId;
        Entity entity = null;
        if (managers.ContainsKey(tgtId))
        {
            entity = managers[tgtId];
        }
        else
        {
            entity = entityManager.GetEntity(tgtId);
        }
        if (entity == null)
        {
            return;
        }

        RpcMethodInfo? rpcMethod = entity.GetRpcMethodInfo(msg.methodName);
        if (rpcMethod == null)
        {
            return;
        }

        object? instance = rpcMethod.GetMethodInstance(entity);
        if (instance == null)
        {
            return;
        }

        MethodInfo method = rpcMethod.methodInfo;
        var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
        if (rpcAttr == null)
        {
            return;
        }

        int rpcType = rpcAttr.rpcType;
        if (rpcType == RpcConst.OwnClient && proxy.eid != entity.eid.Getter())
        {
            return;
        }

        CustomList args = msg.arg.WrapInList();
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
