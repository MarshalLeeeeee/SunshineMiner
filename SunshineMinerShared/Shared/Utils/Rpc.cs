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
    public string fullPath = "";
    public MethodInfo methodInfo;

    public RpcMethodInfo(MethodInfo methodInfo_)
    {
        methodInfo = methodInfo_;
    }

    public RpcMethodInfo(string fullPath_, MethodInfo methodInfo_)
    {
        fullPath = fullPath_;
        methodInfo = methodInfo_;
    }

    public object? GetMethodInstance(Entity entity)
    {
        return entity.GetComponentByFullPath(fullPath);
    }
}

