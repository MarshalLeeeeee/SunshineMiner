using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class SyncConst
{
    public const int OwnClient = (1 << 0);
    public const int OtherClient = (1 << 1);

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

[AttributeUsage(AttributeTargets.Property)]
public class PropertySyncAttribute : Attribute
{
    public int syncType { get; }
    public PropertySyncAttribute(int syncType_)
    {
        syncType = syncType_;
    }
}

