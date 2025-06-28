using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Singleton instance
    public static Game Instance { get; private set; }

    private Dictionary<string, Manager> managers = new Dictionary<string, Manager>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateManager<Gate>();
        CreateManager<EntityManager>();
        CreateManager<EventManager>();
        CreateManager<TimerManager>();
    }

    private void OnEnable()
    {
        StartManagers();
    }

    private void FixedUpdate()
    {
        UpdateManagers();
    }

    private void OnDisable()
    {
        StopManagers();
    }

    #region REGION_MANAGER

    private void CreateManager<T>() where T : Manager, new()
    {
        Type type = typeof(T);
        string name = type.Name;
        T mgr = new T();
        managers[name] = mgr;
    }

    private T? GetManager<T>() where T : Manager
    {
        Type type = typeof(T);
        string name = type.Name;
        if (managers.TryGetValue(name, out Manager manager))
        {
            if (manager != null && manager is T mgr)
            {
                return mgr;
            }
            return null;
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
        method.Invoke(instance, args.ToArray());
    }

    #endregion
}
