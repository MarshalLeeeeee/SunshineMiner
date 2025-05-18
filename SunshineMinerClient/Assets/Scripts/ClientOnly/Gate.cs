using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;

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

    private ConcurrentQueue<Msg> msgs = new ConcurrentQueue<Msg>();

    public Gate(string eid) : base(eid) { }

    /*
     * Instantiate instance
     * Create Tcp client
     */
    public override void Start()
    {
        Debug.Log("Gate Start...");

        client = new TcpClient();
        msgListenerTask = Task.CompletedTask;
        msgListenerCts = new CancellationTokenSource();
        connectState = ConnectState.Disconnected;

        lastHeartbeatTime = 0;
    }

    /*
     * Invoked in main thread update
     */
    public override void Update()
    {
        // check connection
        CheckConnection();
        // handle msgs
        ConsumeQueuedMsg();
        // heartbeat
        PingHeartbeatRemote();
    }

    public override void Stop()
    {
        ResetConnection();
    }

    #region REGION_PUBLIC_UTILITY

    /*
     * Make connection to the server (in main thread)
     */
    public void ConnectedToServer()
    {
        if (IsConnected())
        {
            Debug.Log("Connected already...");
            return;
        }
        if (IsConnecting())
        {
            Debug.Log("Connecting already...");
            return;
        }
        if (IsDisconnecting())
        {
            Debug.Log("Disconnecting, try later...");
            return;
        }
        connectState = ConnectState.Connecting;
        try
        {
            client.Connect("localhost", Const.Port);
            Debug.Log("Client gate connected...");
            StartMsgListener();
        }
        catch (Exception ex)
        {
            connectState = ConnectState.Disconnected;
            Game.Instance.eventManager.TriggerGlobalEvent("GateConnectingOver", false);
            Debug.Log($"Connected to server failed: {ex}");
        }
    }

    public void Login(string account,  string password)
    {
        Msg msg = new Msg("", "AccountManager", "LoginRemote");
        CustomList arg = new CustomList();
        arg.Add(new CustomString(account));
        arg.Add(new CustomString(password));
        msg.arg = arg;
        _ = SendMsgAsync(msg);
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
            Debug.Log("[ResetConnection] Already disconnected or disconnecting...");
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
            Debug.LogError($"Error cleaning up connection: {ex}");
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
        Debug.Log("Connected");
    }

    [Rpc(RpcConst.Server)]
    public void ConnectionLostRemote()
    {
        ResetConnection();
        Debug.Log("Connection Lost");
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
            Console.WriteLine("Proxy msg listener start failed: already exists...");
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
                        Debug.Log($"Invalid connected state in msg listener");
                        break;
                    }

                    if (!IsSocketConnected())
                    {
                        Debug.Log("Lost socket connection in msg listener");
                        break;
                    }

                    var streamRes = GetStream();
                    if (!streamRes.succ)
                    {
                        Debug.Log($"Invalid stream in msg listener");
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
                        Debug.Log($"Invalid message");
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
                    Console.WriteLine($"Invalid message with error: {ex}");
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
            Console.WriteLine($"Error waiting for msg listener to stop: {ex}");
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
        msgs.Enqueue(msg);
    }

    /*
     * Consume and handle msg (in main thread)
     */
    private void ConsumeQueuedMsg()
    {
        int cnt = 0;
        while (cnt < Const.HandleMsgCntPerUpdate && msgs.TryDequeue(out var msg))
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
        Debug.Log($"HandleMsg method name {msg.methodName}");
        Game.Instance.InvokeRpc(msg);
        //string tgtId = msg.tgtId;
        //Entity e = Game.Instance.entityManager.GetEntity(tgtId);
        //if (e == null)
        //{
        //    Debug.Log("Handle Msg: target entity not found");
        //}
        //else
        //{
        //    e.RemoteCall(msg);
        //}
            //switch (msg.methodName)
            //{
            //    case "ConnectionSucc":
            //        ConfirmConnection();
            //        Debug.Log("Connected");
            //        break;
            //    case "ConnectionLost":
            //        ResetConnection();
            //        Debug.Log("Connection Lost");
            //        break;
            //    case "LoginRes":
            //        bool res = ((CustomBool)(((CustomList)(msg.arg))[0])).Getter();
            //        Game.Instance.eventManager.TriggerGlobalEvent("LoginRes", res);
            //        break;
            //}
    }

    /*
     * Send msg async
     */
    public async Task<bool> SendMsgAsync(Msg msg)
    {
        if (!IsConnected())
        {
            Debug.Log("Not connected, cannot send message");
            return false;
        }

        if (!IsSocketConnected())
        {
            Debug.Log("Socket not connected, cannot send message");
            return false;
        }

        try
        {
            var streamRes = GetStream();
            if (!streamRes.succ)
            {
                Debug.Log("Invalid stream in sending msg");
                return false;
            }

            NetworkStream stream = streamRes.stream;
            return await MsgStreamer.WriteMsgToStreamAsync(stream, msg)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Message sending was canceled");
            return false;
        }
        catch (Exception ex)
        {
            Debug.Log($"Error sending message: {ex}");
            ResetConnection();
            return false;
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

        Debug.Log("Ping heartbeat!");
        Msg msg = new Msg("", "Gate", "PingHeartbeatRemote");
        _ = SendMsgAsync(msg);
    }

    #endregion

    #region REGION_LOGIN

    [Rpc(RpcConst.Server, CustomTypeConst.TypeBool)]
    public void LoginResRemote(CustomBool res)
    {
        bool resValue = res.Getter();
        Game.Instance.eventManager.TriggerGlobalEvent("LoginRes", resValue);
    }

    #endregion
}
