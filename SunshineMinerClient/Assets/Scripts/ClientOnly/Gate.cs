using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

internal static class ConnectState
{
    public const int Disconnected = 0;
    public const int Disconnecting = 1;
    public const int Connected = 2;
    public const int Connecting = 3;
}

public class Gate : Manager
{
    public static Gate Instance { get; private set; }

    private TcpClient client;
    private Task msgListenerTask;
    private CancellationTokenSource msgListenerCts;
    private volatile int connectState;

    private long lastHeartbeatTime;

    private ConcurrentQueue<Msg> msgInbox = new ConcurrentQueue<Msg>();
    private ConcurrentQueue<Msg> msgOutbox = new ConcurrentQueue<Msg>();

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<RpcComp>();
    }

    /*
     * Instantiate instance
     * Create Tcp client
     */
    protected override void DoEnableSelf()
    {
        base.DoEnableSelf();

        client = new TcpClient();
        msgListenerTask = Task.CompletedTask;
        msgListenerCts = new CancellationTokenSource();
        connectState = ConnectState.Disconnected;

        lastHeartbeatTime = 0;
    }

    /*
     * Invoked in main thread update
     */
    protected override void DoUpdateSelf()
    {
        base.DoUpdateSelf();
        // check connection
        CheckConnection();
        // handle msgs
        ConsumeMsgInbox();
        ConsumeMsgOutbox();
        // heartbeat
        PingHeartbeatRemote();
    }

    protected override void DoDisableSelf()
    {
        ResetConnection();
        base.DoDisableSelf();
    }

    #region REGION_PUBLIC_UTILITY

    /*
     * Make connection to the server (in main thread)
     */
    public void ConnectedToServer()
    {
        if (IsConnected())
        {
            Debugger.Log("Connected already...");
            return;
        }
        if (IsConnecting())
        {
            Debugger.Log("Connecting already...");
            return;
        }
        if (IsDisconnecting())
        {
            Debugger.Log("Disconnecting, try later...");
            return;
        }
        connectState = ConnectState.Connecting;
        try
        {
            client.Connect("localhost", Const.Port);
            Debugger.Log("Client gate connected...");
            StartMsgListener();
        }
        catch (Exception ex)
        {
            connectState = ConnectState.Disconnected;
            Game.Instance.eventManager.TriggerGlobalEvent("GateConnectingOver", false);
            Debugger.Log($"Connected to server failed: {ex}");
        }
    }

    public void Login(string account,  string password)
    {
        Msg msg = new Msg("AccountManager", "LoginRemote");
        msg.arg.Add(new PropStringNode(account));
        msg.arg.Add(new PropStringNode(password));
        AppendSendMsg(msg);
    }

    #endregion

    #region REGION_GATE_CONNECTIVITY

    /*
     * Get stream with bool res
     */
    private (bool succ, NetworkStream stream) GetStream()
    {
        try
        {
            return (true, client.GetStream());
        }
        catch
        {
            return (false, null);
        }
    }

    /*
     * Check socket connectivity
     */
    private bool IsSocketConnected()
    {
        if (client?.Client == null) return false;

        try
        {
            if (!client.Client.Connected) return false;

            bool isAlive = client.Client.Poll(0, SelectMode.SelectWrite);
            bool hasError = client.Client.Poll(0, SelectMode.SelectError);

            return isAlive && !hasError;
        }
        catch
        {
            return false;
        }
    }

    public bool IsConnected()
    {
        return connectState == ConnectState.Connected;
    }

    public bool IsConnecting()
    {
        return connectState == ConnectState.Connecting;
    }

    public bool IsDisconnected()
    {
        return connectState == ConnectState.Disconnected;
    }

    public bool IsDisconnecting()
    {
        return connectState == ConnectState.Disconnecting;
    }

    /*
     * Confirm connected to the server (in main thread)
     * Only when the connect state is connecting
     */
    private void ConfirmConnection()
    {
        if (IsConnecting())
        {
            connectState = ConnectState.Connected;
            Game.Instance.eventManager.TriggerGlobalEvent("GateConnectingOver", true);
        }
    }

    /*
     * Check connectivity if marked as connected (in main thread)
     * If socket is not connected, reset connection
     */
    private void CheckConnection()
    {
        if (IsConnected())
        {
            if (!IsSocketConnected())
            {
                ResetConnection();
            }
        }
    }

    /*
     * Stop and reset connection (in any thread)
     * Such that new connection can be made
     */
    private void ResetConnection()
    {
        if (IsDisconnected() || IsDisconnecting())
        {
            Debugger.Log("[ResetConnection] Already disconnected or disconnecting...");
        }
        connectState = ConnectState.Disconnecting;
        StopMsgListener();
        StopClient();
    }

    /*
     * Stop client (in any thread)
     */
    private void StopClient()
    {
        try
        {
            client?.Close();
        }
        catch (Exception ex)
        {
            Debugger.Log($"Error cleaning up connection: {ex}");
        }
        finally
        {
            client = new TcpClient();
            connectState = ConnectState.Disconnected;
        }
    }

    [Rpc(RpcConst.Server)]
    public void ConnectionSuccRemote()
    {
        ConfirmConnection();
        Debugger.Log("Connected");
    }

    [Rpc(RpcConst.Server)]
    public void ConnectionLostRemote()
    {
        ResetConnection();
        Debugger.Log("Connection Lost");
    }

    #endregion

    #region REGION_GATE_MSG_LISTENER

    /*
     * Check if msg listener task exists already
     */
    private bool CheckMsgListenerRunning()
    {
        return msgListenerTask != Task.CompletedTask;
    }

    /*
     * Start msg listener (in main thread)
     */
    private void StartMsgListener()
    {
        if (CheckMsgListenerRunning())
        {
            Debugger.Log("Proxy msg listener start failed: already exists...");
            return;
        }
        msgListenerTask = Task.Run(() => MsgListernerWorker(msgListenerCts.Token));
    }

    /*
     * Listen to new msg (in off thread)
     */
    private async Task MsgListernerWorker(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (!IsConnected() && !IsConnecting())
                    {
                        Debugger.Log($"Invalid connected state in msg listener");
                        break;
                    }

                    if (!IsSocketConnected())
                    {
                        Debugger.Log("Lost socket connection in msg listener");
                        break;
                    }

                    var streamRes = GetStream();
                    if (!streamRes.succ)
                    {
                        Debugger.Log($"Invalid stream in msg listener");
                        break;
                    }

                    NetworkStream stream = streamRes.stream;
                    var res = await MsgStreamer.ReadMsgFromStreamAsync(stream, ct).ConfigureAwait(false);
                    if (res.succ)
                    {
                        OnReceiveMsg(res.msg);
                    }
                    else
                    {
                        Debugger.Log($"Invalid message");
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
                    Debugger.Log($"Invalid message with error: {ex}");
                    await Task.Delay(1000, ct).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            ResetConnection();
        }
    }

    /*
     * Stop msg listener (in any thread)
     */
    private void StopMsgListener()
    {
        if (!CheckMsgListenerRunning()) return;
        try
        {
            msgListenerCts.Cancel();
            msgListenerTask.Wait(1000);
        }
        catch (OperationCanceledException)
        { }
        catch (Exception ex)
        {
            Debugger.Log($"Error waiting for msg listener to stop: {ex}");
        }
        finally
        {
            msgListenerCts.Dispose();
            msgListenerTask = Task.CompletedTask;
        }
    }

    #endregion

    #region REGION_GATE_MSG

    /*
     * Receive msg (in off thread)
     * Enqueue to queue for main thread to consume
     */
    private void OnReceiveMsg(Msg msg)
    {
        msgInbox.Enqueue(msg);
    }

    /*
     * Consume and handle received msg (in main thread)
     */
    private void ConsumeMsgInbox()
    {
        int cnt = 0;
        while (cnt < Const.MsgReceiveCntPerUpdate && msgInbox.TryDequeue(out var msg))
        {
            if (msg == null) continue;
            cnt++;
            HandleMsg(msg);
        }
    }

    /*
     * Handle msg from server (in main thread)
     */
    private void HandleMsg(Msg msg)
    {
        Game.Instance.InvokeRpc(msg);
    }

    public void AppendSendMsg(Msg msg)
    {
        msgOutbox.Enqueue(msg);
    }

    /*
     * Consume and handle msg pending for sending (in main thread)
     */
    private void ConsumeMsgOutbox()
    {
        int cnt = 0;
        while (cnt < Const.MsgSendCntPerUpdate && msgOutbox.TryDequeue(out var msg))
        {
            if (msg == null) continue;
            cnt++;
            SendMsg(msg);
        }
    }

    /*
     * Send msg to server (in main thread)
     * If connection is not valid, reset connection
     */
    private void SendMsg(Msg msg)
    {
        if (!IsConnected())
        {
            Debugger.Log("Not connected, cannot send message");
            return;
        }

        if (!IsSocketConnected())
        {
            Debugger.Log("Socket not connected, cannot send message");
            return;
        }

        try
        {
            var streamRes = GetStream();
            if (!streamRes.succ)
            {
                Debugger.Log("Invalid stream in sending msg");
                return;
            }

            NetworkStream stream = streamRes.stream;
            MsgStreamer.WriteMsgToStream(stream, msg);
        }
        catch (OperationCanceledException)
        {
            Debugger.Log("Message sending was canceled");
            return;
        }
        catch (Exception ex)
        {
            Debugger.Log($"Error sending message: {ex}");
            ResetConnection();
            return;
        }
    }

    #endregion

    #region REGION_GATE_HEARTBEAT

    /*
     * Send heart beat ping periodically. (in main thread)
     * Send msg async
     */
    private void PingHeartbeatRemote()
    {
        if (!IsConnected()) return;

        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (now - lastHeartbeatTime < Const.HeartBeatInterval) return;
        lastHeartbeatTime = now;

        Msg msg = new Msg("Gate", "PingHeartbeatRemote");
        AppendSendMsg(msg);
    }

    #endregion

    #region REGION_LOGIN

    [Rpc(RpcConst.Server, PropNodeConst.DataTypeBool)]
    public void LoginResRemote(PropBoolNode res)
    {
        bool resValue = res.GetValue();
        Game.Instance.eventManager.TriggerGlobalEvent("LoginRes", resValue);
    }

    #endregion
}
