
internal class Env
{
    public const bool IsServer = true;
    public const bool IsClient = false;
}

internal class Const : ConstCommon
{
    public const int Port = 41320; // port for the gate service
    public const int TickInterval = 10; // tick interval of Game.Update
    public const int MsgReceiveCntPerUpdate = 1000; // cnt of handled mag in one update
    public const int MsgSendCntPerUpdate = 1000; // cnt of handled mag in one update
    public const int CheckProxyInterval = 1000; // the min time interval of proxy check
    public const int CheckProxyCntPerUpdate = 5; // cnt of proxy checked in one update
    public const int HeartBeatThreshold = 10000; // million second of the longest inactive heartbeat interval
}
