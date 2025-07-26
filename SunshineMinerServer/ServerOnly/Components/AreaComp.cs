
public class AreaComp : AreaCompCommon
{
    [Rpc(RpcConst.OwnClient, PropNodeConst.DataTypeFloat, PropNodeConst.DataTypeFloat, PropNodeConst.DataTypeFloat)]
    public void SyncPositionRemote(PropFloatNode x_, PropFloatNode y_, PropFloatNode z_, Proxy proxy)
    {
        // TODO position validate
        x.SetValue(x_.GetValue());
        y.SetValue(y_.GetValue());
        z.SetValue(z_.GetValue());
    }
}
