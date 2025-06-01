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

    //public Gate gate { get; private set; }
    //public EntityManager entityManager { get; private set; }
    //public EventManager eventManager { get; private set; }
    //public TimerManager timerManager { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateManager<Gate>("Gate");
        CreateManager<EntityManager>("EntityManager");
        CreateManager<EventManager>("EventManager");
        CreateManager<TimerManager>("TimerManager");
    }

    private void OnEnable()
    {
        StartManagers();
    }

    private void Update()
    {
        UpdateManagers();
    }

    private void OnDisable()
    {
        StopManagers();
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

    #endregion

    #region REGION_RPC

    public void InvokeRpc(Msg msg)
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

        method.Invoke(instance, args.ToArray());
    }

    #endregion
}
