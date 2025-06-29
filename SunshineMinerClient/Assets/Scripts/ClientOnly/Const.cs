
internal class Env
{
    public const bool IsServer = false;
    public const bool IsClient = true;
}

internal class Const : ConstCommon
{
    public const int Port = 41320; // port for the gate service
    public const int MsgReceiveCntPerUpdate = 1000; // cnt of handled mag in one update
    public const int MsgSendCntPerUpdate = 1000; // cnt of handled mag in one update
    public const int HeartBeatInterval = 3000; // million second of the heartbeat ping interval
}
