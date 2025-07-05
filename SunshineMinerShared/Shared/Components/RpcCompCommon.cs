using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class RpcCompCommon : Component
{
    protected Dictionary<string, RpcMethodInfo> rpcMethods = new Dictionary<string, RpcMethodInfo>();

    public override void Enable()
    {
        base.Enable();
        EnableRpcMethod();
    }

    public override void Disable()
    {
        DisableRpcMethod();
        base.Disable();
    }

    public RpcMethodInfo? GetRpcMethodInfo(string methodName)
    {
        if (rpcMethods.TryGetValue(methodName, out RpcMethodInfo? rpcMethod))
        {
            return rpcMethod;
        }
        return null;
    }

    protected virtual void EnableRpcMethod()
    {

    }

    protected void EnableRpcMethodWithType(int rpcType)
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
        foreach (KeyValuePair<string, Component> kvp in entity.IterComponents())
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

    protected virtual void EnableCompRpcMethod(Component comp)
    {
        
    }

    protected void EnableCompRpcMethodWithType(Component comp, int rpcType)
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

    protected virtual void DisableCompRpcMethod(Component comp)
    {
        
    }

    protected void DisableCompRpcMethodWithType(Component comp, int rpcType)
    {
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
            rpcMethods.Remove(method.Name);
        }
    }
}
