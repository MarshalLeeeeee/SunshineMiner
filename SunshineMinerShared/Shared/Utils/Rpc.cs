using System;
using System.Reflection;

public class RpcConst
{
    public const int OwnClient = (1 << 0);
    public const int AnyClient = (1 << 1);
    public const int Server = (1 << 2);

    public const int Client = OwnClient | AnyClient;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RpcAttribute : Attribute
{
    public int rpcType { get; }
    public int[] argTypes { get; }

    public RpcAttribute(int rpcType_, params int[] argTypes_)
    {
        rpcType = rpcType_;
        argTypes = argTypes_;
    }
}

public class RpcMethodInfo
{
    public string compName;
    public MethodInfo methodInfo;

    public RpcMethodInfo(MethodInfo methodInfo_)
    {
        compName = "";
        methodInfo = methodInfo_;
    }

    public RpcMethodInfo(string compName_, MethodInfo methodInfo_)
    {
        compName = compName_;
        methodInfo = methodInfo_;
    }

    public object? GetMethodInstance(Entity entity)
    {
        if (compName == "")
        {
            return entity;
        }
        else
        {
            return entity.GetComponentByName(compName);
        }
    }
}

