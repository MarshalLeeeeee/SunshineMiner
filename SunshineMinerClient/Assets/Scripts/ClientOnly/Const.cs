using System;

internal class Env
{
    public const bool IsServer = false;
    public const bool IsClient = true;
}

internal class Const : ConstBase
{
    public const int Port = 41320; // port for the gate service
    public const int HandleMsgCntPerUpdate = 1000; // cnt of handled mag in one update
    public const int HeartBeatInterval = 3000; // million second of the heartbeat ping interval
}
