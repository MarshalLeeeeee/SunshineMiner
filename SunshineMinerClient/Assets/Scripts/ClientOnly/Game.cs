using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Singleton instance
    public static Game Instance { get; private set; }

    public Gate gate { get; private set; }
    public EntityManager entityManager { get; private set; }
    public EventManager eventManager { get; private set; }
    public TimerManager timerManager { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        entityManager = new EntityManager(Guid.NewGuid().ToString());
        gate = entityManager.CreateManager<Gate>("Gate");
        eventManager = entityManager.CreateManager<EventManager>("EventManager");
        timerManager = entityManager.CreateManager<TimerManager>("TimerManager");
    }

    private void OnEnable()
    {
        gate.Start();
        entityManager.Start();
        eventManager.Start();
        timerManager.Start();
    }

    private void OnDisable()
    {
        gate.Stop();
        entityManager.Stop();
        eventManager.Stop();
        timerManager.Stop();
    }

    private void Update()
    {
        gate.Update();
        entityManager.Update();
        eventManager.Update();
        timerManager.Update();
    }

    #region REGION_RPC

    public void InvokeRpc(Msg msg)
    {
        string tgtId = msg.tgtId;
        Entity entity = entityManager.GetEntity(tgtId);
        if (entity == null)
        {
            return;
        }

        var method = entity.GetType().GetMethod(msg.methodName);
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

        method.Invoke(entity, args.ToArray());
    }

    #endregion
}
