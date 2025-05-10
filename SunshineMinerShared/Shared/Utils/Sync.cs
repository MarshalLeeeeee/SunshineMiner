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

[AttributeUsage(AttributeTargets.Property)]
public class SyncTypeAttribute : Attribute
{
    public int syncType { get; }
    public SyncTypeAttribute(int syncType_)
    {
        syncType = syncType_;
    }
}

