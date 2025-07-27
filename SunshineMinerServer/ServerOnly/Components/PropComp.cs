
public class PropComp : PropCompCommon
{
    public override void OnFloatSetter(float old, float n, int syncType, FuncNode? owner, string name)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropFloatSetterRemote");
        msg.arg.Add(new PropFloatNode(n));
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(name));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
}