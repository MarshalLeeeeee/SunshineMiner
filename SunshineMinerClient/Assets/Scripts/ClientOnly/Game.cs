using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Singleton instance
    public static Game Instance { get; private set; }

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

        entityManager = new EntityManager();
        eventManager = new EventManager();
        timerManager = new TimerManager();
    }

    private void OnEnable()
    {
        entityManager.Start();
        eventManager.Start();
        timerManager.Start();
    }

    private void OnDisable()
    {
        entityManager.Stop();
        eventManager.Stop();
        timerManager.Stop();
    }

    private void Update()
    {
        entityManager.Update();
        eventManager.Update();
        timerManager.Update();
    }

    #region REGION_RPC

    public void InvokeRpc(object instance, string callerId, string methodName, CustomList args)
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
