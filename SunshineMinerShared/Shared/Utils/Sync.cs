using System;

public static class SyncConst
{
    public const int OwnClient = (1 << 0);
    private const int OtherClient = (1 << 1);

    public const int AllClient = (OwnClient | OtherClient);
}

[AttributeUsage(AttributeTargets.Class)]
public class EntitySyncAttribute : Attribute
{
    public int syncType { get; }
    public EntitySyncAttribute(int syncType_)
    {
        syncType = syncType_;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class PropertySyncAttribute : Attribute
{
    public int syncType;
    public PropertySyncAttribute(int syncType_)
    {
        syncType = syncType_;
    }
}

