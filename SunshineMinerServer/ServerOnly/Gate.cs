using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


internal class Proxy
{
    public TcpClient client { get; }
    public Guid pid { get; }

    public Proxy(TcpClient client_)
    {
        client = client_;
        pid = Guid.NewGuid();
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

    /*
     * Every proxy listen to msg in a sub thread and save them in a thread safe queue.
     */
    public void Start(Action<Msg> callback)
    {
        new Thread(() =>
        {
            try
            {
                while (true)
                {
                    if (DataStreamer.ReadMsgFromStream(stream, out Msg msg))
                    {
                        callback(msg);
                    }
                    else
                    {
                        Console.WriteLine($"[{GetIp()}::{GetPort()}] Invalid message");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{GetIp()}::{GetPort()}] Receive msg failed: {ex}");
            }
        }).Start();
    }
}

internal class Gate
{
    private TcpListener listener;
    private Task listenerTask;
    private CancellationTokenSource listenerCts;
    private ConcurrentDictionary<Guid, Proxy> proxies;
    private ConcurrentDictionary<Guid, Queue<Msg>> msgs;
    public bool isActive { get; private set; }

    public Gate(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listenerTask = Task.CompletedTask;
        listenerCts = new CancellationTokenSource();
        proxies = new ConcurrentDictionary<Guid, Proxy>();
        msgs = new ConcurrentDictionary<Guid, Queue<Msg>>();
        isActive = false;
    }

    /*
     * Start gate service
     */
    public void Start()
    {
        StartListener();
        isActive = true;
    }

    private bool CheckListenerTaskRunning()
    {
        return listenerTask != Task.CompletedTask && !listenerTask.IsCompleted;
    }

    private void StartListener()
    {
        if (CheckListenerTaskRunning()) return;
        listener.Start();
        listenerTask = Task.Run(() => ListernerWorker(listenerCts.Token));
    }

    private async Task ListernerWorker(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    HandleNewConnection(client);
                }
                catch (ObjectDisposedException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Accept connection failed: {ex}");
                    await Task.Delay(1000, ct).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            listener.Stop();
        }
    }

    private async Task StopListenerAsync()
    {
        if (!CheckListenerTaskRunning()) return;
        listenerCts.Cancel();
        try
        {
            await listenerTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        { }
        catch (Exception ex)
        {
            Console.WriteLine($"Error waiting for listener to stop: {ex}");
        }
        finally
        {
            listenerCts.Dispose();
            listenerCts = new CancellationTokenSource();
            listenerTask = Task.CompletedTask;
        }
    }

    /*
     * Cache client proxy and start proxy service
     */
    private void HandleNewConnection(TcpClient client)
    {
        Console.WriteLine("HandleNewConnection");
        var proxy = new Proxy(client);
        proxies[proxy.pid] = proxy;
        proxy.Start((msg) => OnReceiveMsg(proxy, msg));
        Msg msg = new Msg("", "", "ConnectionSucc");
        SendMsg(proxy, msg);
    }

    /*
     * Invoked in main thread update
     */
    public void Update(float dt)
    {
        // handle queued msgs
        ConcurrentDictionary<Guid, Queue<Msg>> currentMsgs = Interlocked.Exchange(ref msgs, new ConcurrentDictionary<Guid, Queue<Msg>>());
        foreach (var kvp in currentMsgs)
        {
            if (!proxies.ContainsKey(kvp.Key))
            {
                continue;
            }
            Proxy proxy = proxies[kvp.Key];
            while (kvp.Value.Count > 0)
            {
                Msg msg = kvp.Value.Dequeue();
                HandleMsg(proxy, msg);
            }
        }
    }

    /*
     * Cache msg in thread safe containers
     */
    private void OnReceiveMsg(Proxy proxy, Msg msg)
    {
        Guid pid = proxy.pid;
        if (!msgs.ContainsKey(pid))
        {
            msgs[pid] = new Queue<Msg>();
        }
        msgs[pid].Enqueue(msg);
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
        DataStreamer.WriteMsgToStream(proxy.stream, msg);
    }

    public async Task StopAsync()
    {
        isActive = false;
        await StopListenerAsync();
    }

    ~Gate()
    {
        if (isActive)
        {
            StopAsync().Wait(1000);
        }
    }
}

