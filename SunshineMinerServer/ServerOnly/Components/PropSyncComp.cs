using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PropSyncComp: PropSyncCompCommon
{
    protected override Action<PropNode, PropNode>? GetOnSetter(int syncType, string objectName, string name)
    {
        return (o, n) => { OnSetter(o, n, syncType, objectName, name); };
    }

    private void OnSetter(PropNode o, PropNode n, int syncType, string objectName, string name)
    {
        if (entity == null) return;
        // Debugger.Log($"PropSyncComp OnSetter old {o} new {n} syncType {syncType} objectName {objectName} name {name}");
        string eid_ = entity.eid.GetValue();
        Msg msg = new Msg(eid_, "PropSyncSetterRemote");
        msg.arg.Add(n);
        msg.arg.Add(new PropStringNode(objectName));
        msg.arg.Add(new PropStringNode(name));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid_, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
}