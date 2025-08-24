
[Comp]
public class PropComp : PropCompCommon
{
    #region REGION_PROP_SET_CALLBACK

    public override void OnIntSetter(int old, int n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropIntSetterRemote");
        msg.arg.Add(new PropIntNode(n));
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public override void OnFloatSetter(float old, float n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropFloatSetterRemote");
        msg.arg.Add(new PropFloatNode(n));
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public override void OnStringSetter(string old, string n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropStringSetterRemote");
        msg.arg.Add(new PropStringNode(n));
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public override void OnBoolSetter(bool old, bool n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropBoolSetterRemote");
        msg.arg.Add(new PropBoolNode(n));
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public virtual void OnListAssign(int idx, PropNode o, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropListAssignRemote");
        msg.arg.Add(new PropIntNode(idx));
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public override void OnListAdd(PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropListAddRemote");
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public virtual void OnListInsert(int index, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropListInsertRemote");
        msg.arg.Add(new PropIntNode(index));
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public override void OnListRemove(PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropListRemoveRemote");
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public override void OnListClear(int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropListClearRemote");
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public virtual void OnIntDictionaryAssign(int key, PropNode o, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropIntDictionaryAddRemote");
        msg.arg.Add(new PropIntNode(key));
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
    public virtual void OnFloatDictionaryAssign(float key, PropNode o, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropFloatDictionaryAddRemote");
        msg.arg.Add(new PropFloatNode(key));
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
    public virtual void OnStringDictionaryAssign(string key, PropNode o, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropStringDictionaryAddRemote");
        msg.arg.Add(new PropStringNode(key));
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public virtual void OnIntDictionaryAdd(int key, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropIntDictionaryAddRemote");
        msg.arg.Add(new PropIntNode(key));
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
    public virtual void OnFloatDictionaryAdd(float key, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropFloatDictionaryAddRemote");
        msg.arg.Add(new PropFloatNode(key));
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
    public virtual void OnStringDictionaryAdd(string key, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropStringDictionaryAddRemote");
        msg.arg.Add(new PropStringNode(key));
        msg.arg.Add(n.Copy());
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public virtual void OnIntDictionaryRemove(int key, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropIntDictionaryRemoveRemote");
        msg.arg.Add(new PropIntNode(key));
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
    public virtual void OnFloatDictionaryRemove(float key, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropFloatDictionaryRemoveRemote");
        msg.arg.Add(new PropFloatNode(key));
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
    public virtual void OnStringDictionaryRemove(string key, PropNode n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropStringDictionaryRemoveRemote");
        msg.arg.Add(new PropStringNode(key));
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public virtual void OnIntDictionaryClear(int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropIntDictionaryClearRemote");
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
    public virtual void OnFloatDictionaryClear(int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropFloatDictionaryClearRemote");
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }
    public virtual void OnStringDictionaryClear(int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropStringDictionaryClearRemote");
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    public virtual void OnVector3Setter(Vec3 o, Vec3 n, int syncType, FuncNode? owner, string propRootName, string propFullHash)
    {
        if (entity == null) return;
        if (owner == null) return;
        string eid = entity.eid.GetValue();
        Msg msg = new Msg(eid, "PropVector3SetterRemote");
        msg.arg.Add(new PropVector3Node(n));
        string ownerFullPath = owner.fullPath;
        msg.arg.Add(new PropStringNode(ownerFullPath));
        msg.arg.Add(new PropStringNode(propRootName));
        msg.arg.Add(new PropStringNode(propFullHash));
        if (syncType == SyncConst.OwnClient)
        {
            Game.Instance.gate.RpcToOwnClient(eid, msg);
        }
        else if (syncType == SyncConst.AllClient)
        {
            Game.Instance.gate.RpcToAllClient(msg);
        }
    }

    #endregion
}