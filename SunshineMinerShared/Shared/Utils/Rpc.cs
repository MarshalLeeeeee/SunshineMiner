using System;

public class RpcConst
{
    public const int OwnClient = (1 << 0);
    public const int AnyClient = (1 << 1);
    public const int Server = (1 << 2);
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

