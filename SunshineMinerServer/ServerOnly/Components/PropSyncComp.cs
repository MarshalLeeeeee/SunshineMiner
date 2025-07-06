using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PropSyncComp: PropSyncCompCommon
{
    protected override Action<CustomType, CustomType>? GetOnSetter(int syncType, string objectName, string name)
    {
        return (o, n) => { OnSetter(o, n, syncType, objectName, name); };
    }

    private void OnSetter(CustomType o, CustomType n, int syncType, string objectName, string name)
    {
        if (entity == null) return;
        Debugger.Log($"PropSyncComp OnSetter old {o.CustomToString()} new {n.CustomToString()}");
        if (syncType == SyncConst.OwnClient)
        {
            string eid_ = entity.eid.Getter();
            Msg msg = new Msg(eid_, "PropSyncSetterRemote");
            msg.arg.Add(n);
            msg.arg.Add(new CustomString(objectName));
            msg.arg.Add(new CustomString(name));
            Game.Instance.gate.RpcToOwnClient(eid_, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            string eid_ = entity.eid.Getter();
            Msg msg = new Msg(eid_, "PropSyncSetterRemote");
            msg.arg.Add(n);
            msg.arg.Add(new CustomString(objectName));
            msg.arg.Add(new CustomString(name));
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
}