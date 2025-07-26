
public class AreaComp : AreaCompCommon
{
    [Rpc(RpcConst.OwnClient, DataNodeConst.DataTypeFloat, DataNodeConst.DataTypeFloat, DataNodeConst.DataTypeFloat)]
    public void SyncPositionRemote(DataFloatNode x_, DataFloatNode y_, DataFloatNode z_, Proxy proxy)
    {
        // TODO position validate
        x.SetValue(x_.GetValue());
        y.SetValue(y_.GetValue());
        z.SetValue(z_.GetValue());
    }
}
