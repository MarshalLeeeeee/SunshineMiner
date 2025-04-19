using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;


internal class Proxy
{
    public TcpClient client { get; }

    public Proxy(TcpClient client_)
    {
        client = client_;
    }

    public NetworkStream stream => client.GetStream();

    public string GetIp()
    {
        var clientSocket = client.Client;
        if (clientSocket == null) return "";
        if (clientSocket.RemoteEndPoint == null) return "";
        IPEndPoint clientEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
        if (clientEndPoint == null) return "";
        else return clientEndPoint.Address.ToString();
    }
    public int GetPort()
    {
        var clientSocket = client.Client;
        if (clientSocket == null) return 0;
        if (clientSocket.RemoteEndPoint == null) return 0;
        IPEndPoint clientEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
        if (clientEndPoint == null) return 0;
        else return clientEndPoint.Port;
    }

    public void Receive(Action<Msg> callback)
    {
        new Thread(() =>
        {
            try
            {
                while (true)
                {
                    byte[] lengthBuffer = new byte[4];
                    int lengthBytesRead = 0;
                    while (lengthBytesRead < 4)
                    {
                        int read = stream.Read(lengthBuffer, lengthBytesRead, 4 - lengthBytesRead);
                        if (read == 0) break;
                        lengthBytesRead += read;
                    }
                    if (lengthBytesRead < 4)
                    {
                        Console.WriteLine("Invalid header length");
                        continue;
                    }

                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                    byte[] messageBuffer = new byte[messageLength];
                    int totalBytesRead = 0;
                    while (totalBytesRead < messageLength)
                    {
                        int read = stream.Read(messageBuffer, totalBytesRead, messageLength - totalBytesRead);
                        if (read == 0) break;
                        totalBytesRead += read;
                    }
                    if (totalBytesRead < messageLength)
                    {
                        Console.WriteLine("Incomplete message received");
                        continue;
                    }

                    Msg msg = DataStreamer.Deserialize(messageBuffer);
                    callback(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receiving failed: {ex}");
            }
        }).Start();
    }
}

internal class Gate
{
    private TcpListener listener;
    private ConcurrentBag<Proxy> proxies = new ConcurrentBag<Proxy>();

    public Gate(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
    }

    public void Start()
    {
        listener.Start();
        new Thread(() =>
        {
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                HandleNewConnection(client);
            }
        }).Start();
    }

    private void HandleNewConnection(TcpClient client)
    {
        Console.WriteLine("HandleNewConnection");
        var proxy = new Proxy(client);
        proxies.Add(proxy);
        proxy.Receive((msg) =>
        {
            HandleMsg(proxy, msg);
        });
    }

    static private void HandleMsg(Proxy proxy, Msg msg)
    {
        Console.WriteLine($"HandleMsg method name {msg.methodName} from ip {proxy.GetIp()}::{proxy.GetPort()}");
        Msg response = new Msg("", "", "TestRpcResponse");
        foreach (var kvp in msg.args)
        {
            switch (kvp.Value.type)
            {
                case 1:
                    int objI = (int)(kvp.Value.obj);
                    Console.WriteLine($"{kvp.Key}: {objI}");
                    response.AddArgInt("ResponseInt", objI + 10);
                    break;
                case 2:
                    float objF = (float)(kvp.Value.obj);
                    Console.WriteLine($"{kvp.Key}: {objF}");
                    response.AddArgFloat("ResponseFloat", objF + 10f);
                    break;
                case 3:
                    string objS = (string)(kvp.Value.obj);
                    Console.WriteLine($"{kvp.Key}: {objS}");
                    response.AddArgString("ResponseString", objS + " from server");
                    break;
            }
        }
        SendMsg(proxy, response);
    }

    static private void SendMsg(Proxy proxy, Msg msg)
    {
        byte[] buffer = DataStreamer.Serialize(msg);
        byte[] lengthPrefix = BitConverter.GetBytes(buffer.Length);
        proxy.stream.Write(lengthPrefix, 0, 4);
        proxy.stream.Write(buffer, 0, buffer.Length);
        proxy.stream.Flush();
    }
}

