
public class AreaComp : AreaCompCommon
{
    [Rpc(RpcConst.OwnClient, SyncDataConst.DataTypeFloat, SyncDataConst.DataTypeFloat, SyncDataConst.DataTypeFloat)]
    public void SyncPositionRemote(SyncDataFloatNode x_, SyncDataFloatNode y_, SyncDataFloatNode z_, Proxy proxy)
    {
        // TODO position validate
        x.SetValue(x_.GetValue());
        y.SetValue(y_.GetValue());
        z.SetValue(z_.GetValue());
    }
}
