using System.Collections;
using System.Collections.Generic;

public class AreaComp : AreaCompBase
{
    [Rpc(RpcConst.OwnClient, CustomTypeConst.TypeFloat, CustomTypeConst.TypeFloat, CustomTypeConst.TypeFloat)]
    public void SyncPositionRemote(CustomFloat x_, CustomFloat y_, CustomFloat z_, Proxy proxy)
    {
        // TODO position validate
        x.Setter(x_.Getter());
        y.Setter(y_.Getter());
        z.Setter(z_.Getter());
    }
}
