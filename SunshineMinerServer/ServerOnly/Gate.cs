using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;


internal class Proxy
{
    public Guid pid { get; }
    public long lastHeartbeatTime;
    public long lastCheckTime { get; set; }

    private TcpClient client;
    private Task msgListenerTask;
    private CancellationTokenSource msgListenerCts;

    private volatile bool isActive;


    public Proxy(TcpClient client_)
    {
        pid = Guid.NewGuid();
        lastHeartbeatTime = 0;
        lastCheckTime = 0;

        client = client_;
        msgListenerTask = Task.CompletedTask;
        msgListenerCts = new CancellationTokenSource();

        isActive = false;
    }

    public NetworkStream stream => client.GetStream();

    /*
     * Get ip string
     * Return empty string when invalid
     */
    public string GetIp()
    {
        var clientSocket = client.Client;
        if (clientSocket == null) return "";
        if (clientSocket.RemoteEndPoint == null) return "";
        IPEndPoint clientEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
        if (clientEndPoint == null) return "";
        else return clientEndPoint.Address.ToString();
    }

    /*
     * Get port int
     * Return 0 when invalid
     */
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
     * Start proxy service (in off thread)
     */
    public void Start(Action<Msg> onReceiveMsgCallback, Action<Guid> onDisconnectCallback)
    {
        StartMsgListener(onReceiveMsgCallback, onDisconnectCallback);
        lastHeartbeatTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        isActive = true;
    }

    /*
     * Stop proxy service (in off thread)
     */
    public void Stop()
    {
        if (isActive) return;
        isActive = false;
        StopMsgListener();
    }

    #region REGION_PROXY_CONNECTIVITY

    /*
     * Get if the proxy is connected
     */
    public bool IsConnected()
    {
        if (!isActive)
        {
            return false;
        }
        if (client?.Client == null)
        {
            return false;
        }
        try
        {
            return !(client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0);
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region REGION_PROXY_MSG_LISTENER

    /*
     * Check if msg listener task exists already
     */
    private bool CheckMsgListenerRunning()
    {
        return msgListenerTask != Task.CompletedTask;
    }

    /*
     * Start msg listener (in off thread)
     */
    private void StartMsgListener(Action<Msg> onReceiveMsgCallback, Action<Guid> onDisconnectCallback)
    {
        if (CheckMsgListenerRunning())
        {
            Console.WriteLine("Proxy msg listener start failed: already exists...");
            return;
        }
        msgListenerTask = Task.Run(() => MsgListernerWorker(onReceiveMsgCallback, onDisconnectCallback, msgListenerCts.Token));

    }

    /*
     * Listen to new msg (in off thread)
     */
    private async Task MsgListernerWorker(Action<Msg> onReceiveMsgCallback, Action<Guid> onDisconnectCallback, CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (!IsConnected())
                    {
                        Console.WriteLine($"Proxy [{pid}] [{GetIp()}::{GetPort()}] lost connection in msg listener");
                        break;
                    }

                    var res = await MsgStreamer.ReadMsgFromStreamAsync(stream, ct).ConfigureAwait(false);
                    if (res.succ)
                    {
                        onReceiveMsgCallback(res.msg);
                    }
                    else
                    {
                        Console.WriteLine($"Proxy [{pid}] [{GetIp()}::{GetPort()}] Invalid message");
                        await Task.Delay(1000, ct).ConfigureAwait(false);
                    }
                }
                catch (ObjectDisposedException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
                {
                    break;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Proxy [{pid}] [{GetIp()}::{GetPort()}] Invalid message with error: {ex}");
                    await Task.Delay(1000, ct).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            onDisconnectCallback(pid);
            Stop();
        }
    }

    /*
     * Stop msg listener
     */
    private void StopMsgListener()
    {
        if (!CheckMsgListenerRunning()) return;
        msgListenerCts.Cancel();
        try
        {
            msgListenerTask.Wait(1000);
        }
        catch (OperationCanceledException)
        { }
        catch (Exception ex)
        {
            Console.WriteLine($"Error waiting for msg listener to stop: {ex}");
        }
        finally
        {
            msgListenerCts.Dispose();
            msgListenerTask = Task.CompletedTask;
        }
    }

    #endregion
}

internal class Gate
{
    private TcpListener listener;
    private Task listenerTask;
    private CancellationTokenSource listenerCts;

    private ConcurrentDictionary<Guid, Proxy> proxies; // thread shared
    private ConcurrentQueue<(Guid pid, Msg msg)> msgs; // thread shared
    private ConcurrentQueue<Guid> checkProxyQueue; // thread shared
    private volatile bool isActive;// thread shared

    private long lastCheckTime;

    public Gate()
    {
        listener = new TcpListener(IPAddress.Any, Const.Port);
        listenerTask = Task.CompletedTask;
        listenerCts = new CancellationTokenSource();

        proxies = new ConcurrentDictionary<Guid, Proxy>();
        msgs = new ConcurrentQueue<(Guid pid, Msg msg)>();
        checkProxyQueue = new ConcurrentQueue<Guid>();
        isActive = false;

        lastCheckTime = 0;
    }

    /*
     * Start gate service (in main thread)
     * * Start listener to handle new connections in off thread
     */
    public void Start()
    {
        Console.WriteLine("Gate starts...");
        StartListener();
        isActive = true;
    }

    /*
     * Stop gate service (in main thread)
     * * Stop listener to handle new connections
     */
    public void Stop()
    {
        if (!isActive) return; 
        Console.WriteLine("Gate is shutting down...");
        isActive = false;
        StopListener();
        Console.WriteLine("Gate stop over...");
    }

    /*
     * Invoked in every server tick (in main thread)
     */
    public void Update()
    {
        if (!isActive) return;
        // handle queued msgs
        HandleQueuedMsg();
        // Check validness of some proxies
        CheckProxies();
    }

    #region REGION_LISTENER

    /*
     * Check if listener task exists already
     */
    private bool CheckListenerTaskRunning()
    {
        return listenerTask != Task.CompletedTask;
    }

    /*
     * Start listerner (in main thread)
     */
    private void StartListener()
    {
        if (CheckListenerTaskRunning())
        {
            Console.WriteLine("Start gate listener failed: listener is running already...");
            return;
        }
        listenerTask = Task.Run(() => ListernerWorker(listenerCts.Token));
    }

    /*
     * Start listerner and handle new client async (in off thread)
     */
    private async Task ListernerWorker(CancellationToken ct)
    {
        try
        {
            listener.Start();
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    _ = Task.Run(() => HandleNewConnectionAsync(client));
                }
                catch (ObjectDisposedException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
                {
                    break;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
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

    /*
     * Stop listener (in main thread)
     */
    private void StopListener()
    {
        if (!CheckListenerTaskRunning()) return;
        listenerCts.Cancel();
        try
        {
            listenerTask.Wait(1000);
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
            listenerTask = Task.CompletedTask;
        }
    }

    /*
     * Add client proxy and start proxy service (in off thread)
     */
    private void HandleNewConnectionAsync(TcpClient client)
    {
        if (!isActive) return;
        Console.WriteLine("HandleNewConnection");
        var proxy = new Proxy(client);
        AddProxy(proxy);
    }

    #endregion

    #region REGION_PROXY

    /*
     * Add proxy to be managed (in off thread)
     */
    private bool AddProxy(Proxy proxy)
    {
        if (proxies.TryAdd(proxy.pid, proxy))
        {
            proxy.Start(
                (msg) => OnReceiveMsg(proxy, msg),
                OnProxyDisconnect
            );
            Msg msg = new Msg("", "", "ConnectionSucc");
            Task.Run(() => SendMsgAsync(proxy, msg));
            PushToCheckProxyQueue(proxy.pid);
            Console.WriteLine($"Proxy [{proxy.pid}] is added {proxy.IsConnected()}");
            return true;
        }
        else
        {
            Console.WriteLine($"Proxy {proxy.pid} fails to be added: already managed");
            return false;
        }
    }

    /*
     * Remove proxy from management (in any thread)
     */
    private bool RemoveProxy(Guid pid)
    {
        if (proxies.TryRemove(pid, out var proxy))
        {
            if (proxy != null)
            {
                Msg msg = new Msg("", "", "ConnectionLost");
                Task.Run(() => SendMsgAsync(proxy, msg));
                proxy.Stop();
                Console.WriteLine($"Proxy [{pid}] is removed");
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
     * proxy disconnects internally (in off thread)
     */
    private void OnProxyDisconnect(Guid pid)
    {
        RemoveProxy(pid);
    }

    #endregion

    #region REGION_HEARTBEAT

    /*
     * Push proxy to the check queue, when a new proxy is managed (in off thread)
     */
    private void PushToCheckProxyQueue(Guid pid)
    {
        checkProxyQueue.Enqueue(pid);
    }

    /*
     * Check if some proxies in the queue is alive and valid
     */
    private void CheckProxies()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (now - lastCheckTime < Const.CheckProxyInterval) return;
        lastCheckTime = now;

        int cnt = 0;
        while (cnt < Const.CheckProxyCntPerUpdate && checkProxyQueue.TryDequeue(out Guid pid))
        {
            if (proxies.TryGetValue(pid, out var proxy))
            {
                cnt += 1;
                if (proxy == null)
                {
                    Console.WriteLine($"Check proxy [{pid}]: null");
                    RemoveProxy(pid);
                    continue;
                }
                // check aliveness
                if (!proxy.IsConnected())
                {
                    Console.WriteLine($"Check proxy [{pid}]: disconnect");
                    RemoveProxy(pid);
                    continue;
                }
                if (now - proxy.lastHeartbeatTime > Const.HeartBeatThreshold)
                {
                    Console.WriteLine($"Check proxy [{pid}]: heartbeat expired");
                    RemoveProxy(pid);
                    continue;
                }
                // still valid, push back to check queue
                long prevCheckTime = proxy.lastCheckTime;
                proxy.lastCheckTime = now;
                checkProxyQueue.Enqueue(pid);
                if (Math.Abs(prevCheckTime - now) < 1000) break; // break when cycle in pop-push queue happens
            }
        }
    }

    #endregion

    #region REGION_MSG

    /*
     * Send msg async (in off thread)
     */
    static public async Task SendMsgAsync(Proxy proxy, Msg msg)
    {
        if (!proxy.IsConnected()) return;
        await MsgStreamer.WriteMsgToStreamAsync(proxy.stream, msg);
    }

    /*
     * Cache msg in thread safe containers for main thread to consume (in off thread)
     */
    private void OnReceiveMsg(Proxy proxy, Msg msg)
    {
        Guid pid = proxy.pid;
        msgs.Enqueue((pid, msg));
    }

    /*
     * Consume and hangle msg from queue (in main thread)
     */
    private void HandleQueuedMsg()
    {
        int cnt = 0;
        while (cnt < Const.HangleMsgCntPerUpdate && msgs.TryDequeue(out var item))
        {
            Guid pid = item.pid;
            if (!proxies.TryGetValue(pid, out var proxy)) continue;
            if (proxy == null) continue;
            HandleMsg(proxy, item.msg);
            cnt += 1;
        }
    }

    /*
     * Handle msg with certain proxy (in main thread)
     */
    static private void HandleMsg(Proxy proxy, Msg msg)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Console.WriteLine($"HandleMsg method name {msg.methodName} from ip {proxy.GetIp()}::{proxy.GetPort()}");
        switch (msg.methodName)
        {
            case "PingHeartbeat":
                Console.WriteLine($"Proxy [{proxy.pid}] heartbeat pinged");
                proxy.lastHeartbeatTime = now;
                break;
            case "Login":
                string account = ((CustomString)(((CustomList)(msg.arg))[0])).Getter();
                string password = ((CustomString)(((CustomList)(msg.arg))[1])).Getter();
                Game.Instance.accountManager.Login(proxy, account, password);
                break;
        }
    }

    #endregion
}

