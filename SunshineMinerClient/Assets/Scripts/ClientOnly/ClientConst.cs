using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal static class ClientConst
{
    public const int Port = 41320; // port for the gate service
    public const int HangleMsgCntPerUpdate = 1000; // cnt of handled mag in one update
    public const int HeartBeatInterval = 3000; // million second of the heartbeat ping interval
}
