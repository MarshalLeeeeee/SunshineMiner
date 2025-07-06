using System.Reflection;

public class Game : GameCommon
{
    // Singleton instance
    public static Game Instance { get; private set; } = null!;

    public Game()
    {
        Instance = this;
        InitManagers();
    }

    #region REGION_MANAGER

    protected override void InitManagers()
    {
        CreateManager<Gate>();
        CreateManager<EntityManager>();
        CreateManager<EventManager>();
        CreateManager<TimerManager>();
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

    #endregion

    #region REGION_RPC

    public void InvokeRpc(Msg msg)
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

        RpcComp? rpcComp = entity.GetComponent<RpcComp>();
        if (rpcComp == null)
        {
            return;
        }

        RpcMethodInfo? rpcMethod = rpcComp.GetRpcMethodInfo(msg.methodName);
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

        CustomList args = msg.arg;

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
            if (rpcArgs[i] != CustomTypeConst.TypeUndefined && arg.type != rpcArgs[i])
            {
                return;
            }
            i += 1;
        }
        method.Invoke(instance, args.ToArray());
    }

    #endregion
}
