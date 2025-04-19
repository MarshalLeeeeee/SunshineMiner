using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;
using System.Text.Json;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public static Gate Instance { get; private set; }

    private TcpClient client;
    private NetworkStream stream => client.GetStream();

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

        client = new TcpClient();
        client.Connect("localhost", 41320);
        Debug.Log("Client gate connected...");
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
                    HandleMsg(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receiving failed: {ex}");
            }
        }).Start();
    }

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
        byte[] buffer = DataStreamer.Serialize(msg);
        byte[] lengthPrefix = BitConverter.GetBytes(buffer.Length);
        Debug.Log($"Buffer length {buffer.Length}");
        stream.Write(lengthPrefix, 0, 4);
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
