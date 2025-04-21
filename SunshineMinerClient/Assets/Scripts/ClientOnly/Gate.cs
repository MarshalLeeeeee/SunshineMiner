using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections.Concurrent;

public class Gate : MonoBehaviour
{
    public static Gate Instance { get; private set; }

    private TcpClient client;
    private NetworkStream stream => client.GetStream();
    private ConcurrentQueue<Msg> msgs = new ConcurrentQueue<Msg>();

    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("Gate awake...");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectedToServer()
    {
        client = new TcpClient();
        client.Connect("localhost", 41320);
        Debug.Log("Client gate connected...");
        new Thread(() =>
        {
            try
            {
                while (true)
                {
                    if (DataStreamer.ReadMsgFromStream(stream, out Msg msg))
                    {
                        OnReceiveMsg(msg);
                    }
                    else
                    {
                        Debug.Log("Invalid message");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Receiving failed: {ex}");
            }
        }).Start();
    }

    private void OnReceiveMsg(Msg msg)
    {
        msgs.Enqueue(msg);
    }

    /*
     * Invoked in main thread update
     */
    void Update()
    {
        // handle msgs
        ConcurrentQueue<Msg> currentMsgs = Interlocked.Exchange(ref msgs, new ConcurrentQueue<Msg>());
        while (currentMsgs.TryDequeue(out Msg msg))
        {
            HandleMsg(msg);
        }
    }

    /*
     * Handle msg from server in main thread
     */
    private void HandleMsg(Msg msg)
    {
        Debug.Log($"HandleMsg method name {msg.methodName}");
        foreach (var kvp in msg.args)
        {
            switch (kvp.Value.type)
            {
                case 1:
                    int objI = (int)(kvp.Value.obj);
                    Debug.Log($"{kvp.Key}: {objI}");
                    break;
                case 2:
                    float objF = (float)(kvp.Value.obj);
                    Debug.Log($"{kvp.Key}: {objF}");
                    break;
                case 3:
                    string objS = (string)(kvp.Value.obj);
                    Debug.Log($"{kvp.Key}: {objS}");
                    break;
            }
        }
    }

    public void SendMsg(Msg msg)
    {
        DataStreamer.WriteMsgToStream(stream, msg);
    }
}
