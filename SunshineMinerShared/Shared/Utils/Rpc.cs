using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RpcAttribute : Attribute
{
    public int[] parameterTypes { get; }

    public RpcAttribute(params int[] parameterTypes_)
    {
        parameterTypes = parameterTypes_;
    }
}

public class Rpc
{
    /*
     * Validate if method is a rpc method
     * Validate if args are valid
     */
}
