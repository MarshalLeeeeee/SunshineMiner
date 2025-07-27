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
    }

    protected override void DoDisableSelf()
    {
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
        if (entity != null)
        {
            DoEnableRpcRecursive(entity);
            string eid = entity.eid.GetValue();
            Game.Instance.eventManager.RegisterEntityEvent<Component>(eid, "EnableComponent", "DoEnableRpc", DoEnableRpc);
            Game.Instance.eventManager.RegisterEntityEvent<Component>(eid, "DisableComponent", "DoDisableRpc", DoDisableRpc);
            Game.Instance.eventManager.RegisterEntityEvent<Component>(eid, "EnableEntity", "DoEnableRpc", DoEnableRpc);
            Game.Instance.eventManager.RegisterEntityEvent<Component>(eid, "DisableEntity", "DoDisableRpc", DoDisableRpc);
        }
    }

    protected void DoEnableRpcRecursive(Component node)
    {
        DoEnableRpc(node);
        foreach (KeyValuePair<string, Component> kvp in node.IterComponents())
        {
            Component comp = kvp.Value;
            DoEnableRpcRecursive(comp);
        }
    }

    protected void DoEnableRpc(Component node)
    {
        if (!node.enabled) return;
        int rpcType = GetRpcType();
        Type type = node.GetType();
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
            rpcMethods[method.Name] = new RpcMethodInfo(node.fullPath, method);
        }
    }

    protected void DisableRpcMethod()
    {
        if (entity != null)
        {
            string eid = entity.eid.GetValue();
            Game.Instance.eventManager.UnregisterEntityEvent(eid, "EnableComponent", "DoEnableRpc");
            Game.Instance.eventManager.UnregisterEntityEvent(eid, "DisableComponent", "DoDisableRpc");
            Game.Instance.eventManager.UnregisterEntityEvent(eid, "EnableEntity", "DoEnableRpc");
            Game.Instance.eventManager.UnregisterEntityEvent(eid, "DisableEntity", "DoDisableRpc");
            DoDisableRpcRecursive(entity);
        }
    }

    protected void DoDisableRpcRecursive(Component node)
    {
        DoDisableRpc(node);
        foreach (KeyValuePair<string, Component> kvp in node.IterComponents())
        {
            Component comp = kvp.Value;
            DoDisableRpcRecursive(comp);
        }
    }

    protected void DoDisableRpc(Component node)
    {
        if (node.enabled) return;
        int rpcType = GetRpcType();
        Type type = node.GetType();
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
            rpcMethods.Remove(method.Name);
        }
    }
}
