using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class RpcCompCommon : Component
{
    protected Dictionary<string, RpcMethodInfo> rpcMethods = new Dictionary<string, RpcMethodInfo>();

    protected override void DoEnableSelf()
    {
        base.DoEnableSelf();
        EnableRpcMethod();
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.GetValue(), "EnableComponent", "EnableCompRpcMethod", EnableCompRpcMethod);
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.GetValue(), "DisableComponent", "DisableCompRpcMethod", DisableCompRpcMethod);
    }

    protected override void DoDisableSelf()
    {
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.GetValue(), "EnableComponent", "EnableCompRpcMethod");
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.GetValue(), "DisableComponent", "DisableCompRpcMethod");
        DisableRpcMethod();
        base.DoDisableSelf();
    }

    public RpcMethodInfo? GetRpcMethodInfo(string methodName)
    {
        if (!enabled || entity == null)
        {
            return null;
        }
        if (rpcMethods.TryGetValue(methodName, out RpcMethodInfo? rpcMethod))
        {
            return rpcMethod;
        }
        return null;
    }

    protected virtual int GetRpcType()
    {
        return 0;
    }

    protected void EnableRpcMethod()
    {
        EnableRpcMethodWithType(GetRpcType());
    }

    private void EnableRpcMethodWithType(int rpcType)
    {
        if (entity == null) return;

        // rpc method from entity
        Type type = entity.GetType();
        var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (MethodInfo method in methods)
        {
            var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
            if (rpcAttr == null)
            {
                continue;
            }
            if ((rpcAttr.rpcType & rpcType) == 0)
            {
                continue;
            }
            rpcMethods[method.Name] = new RpcMethodInfo(method);
        }


        // rpc method from inited components
        foreach (KeyValuePair<string, Component> kvp in entity.IterComponents()) // TODO: nested components
        {
            string compName = kvp.Key;
            Component comp = kvp.Value;
            Type compType = comp.GetType();
            var compMethods = compType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (MethodInfo method in compMethods)
            {
                var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
                if (rpcAttr == null)
                {
                    continue;
                }
                if ((rpcAttr.rpcType & rpcType) == 0)
                {
                    continue;
                }
                rpcMethods[method.Name] = new RpcMethodInfo(compName, method);
            }
        }
    }

    protected void DisableRpcMethod()
    {
        rpcMethods.Clear();
    }

    protected void EnableCompRpcMethod(Component comp)
    {
        EnableCompRpcMethodWithType(comp, GetRpcType());
    }

    private void EnableCompRpcMethodWithType(Component comp, int rpcType)
    {
        Type compType = comp.GetType();
        string compName = compType.Name;
        var compMethods = compType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (MethodInfo method in compMethods)
        {
            var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
            if (rpcAttr == null)
            {
                continue;
            }
            if ((rpcAttr.rpcType & rpcType) == 0)
            {
                continue;
            }
            rpcMethods[method.Name] = new RpcMethodInfo(compName, method);
        }
    }

    protected void DisableCompRpcMethod(Component comp)
    {
        DisableCompRpcMethodWithType(comp, GetRpcType());
    }

    private void DisableCompRpcMethodWithType(Component comp, int rpcType)
    {
        Type compType = comp.GetType();
        string compName = compType.Name;
        var compMethods = compType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (MethodInfo method in compMethods)
        {
            var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
            if (rpcAttr == null)
            {
                continue;
            }
            if ((rpcAttr.rpcType & rpcType) == 0)
            {
                continue;
            }
            rpcMethods.Remove(method.Name);
        }
    }
}
