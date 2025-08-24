using System;
using System.Reflection;

[Comp]
public class PropComp : PropCompCommon
{
    #region REGION_PROP_REMOTE_SET

    [Rpc(RpcConst.Server, PropNodeConst.TypeInt, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropIntSetterRemote(PropIntNode value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropIntNode? pin = entity.GetPropNode<PropIntNode>(ownerFullPath, propRootName, propFullHash);
        pin?.SetValue(value.GetValue());
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeFloat, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropFloatSetterRemote(PropFloatNode value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropFloatNode? pfn = entity.GetPropNode<PropFloatNode>(ownerFullPath, propRootName, propFullHash);
        pfn?.SetValue(value.GetValue());
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropStringSetterRemote(PropStringNode value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropStringNode? psn = entity.GetPropNode<PropStringNode>(ownerFullPath, propRootName, propFullHash);
        psn?.SetValue(value.GetValue());
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeBool, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropBoolSetterRemote(PropBoolNode value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropBoolNode? pbn = entity.GetPropNode<PropBoolNode>(ownerFullPath, propRootName, propFullHash);
        pbn?.SetValue(value.GetValue());
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeInt, PropNodeConst.TypeUndefined, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropListAssignRemote(PropIntNode idx_, PropNode value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropListNode? pln = entity.GetPropNode<PropListNode>(ownerFullPath, propRootName, propFullHash);
        if (pln == null) return;
        int idx = idx_.GetValue();
        pln[idx] = value;
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeUndefined, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropListAddRemote(PropNode value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropListNode? pln = entity.GetPropNode<PropListNode>(ownerFullPath, propRootName, propFullHash);
        pln?.Add(value);
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeInt, PropNodeConst.TypeUndefined, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropListInsertRemote(PropIntNode idx_, PropNode value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        int idx = idx_.GetValue();
        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropListNode? pln = entity.GetPropNode<PropListNode>(ownerFullPath, propRootName, propFullHash);
        pln?.Insert(idx, value);
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeUndefined, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropListRemoveRemote(PropNode value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropListNode? pln = entity.GetPropNode<PropListNode>(ownerFullPath, propRootName, propFullHash);
        pln?.Remove(value);
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropListClearRemote(PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropListNode? pln = entity.GetPropNode<PropListNode>(ownerFullPath, propRootName, propFullHash);
        pln?.Clear();
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeInt, PropNodeConst.TypeUndefined, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropIntDictionaryAddRemote(PropIntNode key, PropNode n, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropIntDictionaryNode? pidn = entity.GetPropNode<PropIntDictionaryNode>(ownerFullPath, propRootName, propFullHash);
        pidn?.Add(key.GetValue(), n);
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeFloat, PropNodeConst.TypeUndefined, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropFloatDictionaryAddRemote(PropFloatNode key, PropNode n, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropFloatDictionaryNode? pfdn = entity.GetPropNode<PropFloatDictionaryNode>(ownerFullPath, propRootName, propFullHash);
        pfdn?.Add(key.GetValue(), n);
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeString, PropNodeConst.TypeUndefined, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropStringDictionaryAddRemote(PropStringNode key, PropNode n, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropStringDictionaryNode? psdn = entity.GetPropNode<PropStringDictionaryNode>(ownerFullPath, propRootName, propFullHash);
        psdn?.Add(key.GetValue(), n);
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeInt, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropIntDictionaryRemoveRemote(PropIntNode key, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        int k = key.GetValue();
        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropIntDictionaryNode? pidn = entity.GetPropNode<PropIntDictionaryNode>(ownerFullPath, propRootName, propFullHash);
        pidn?.Remove(k);
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeFloat, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropFloatDictionaryRemoveRemote(PropFloatNode key, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        float k = key.GetValue();
        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropFloatDictionaryNode? pfdn = entity.GetPropNode<PropFloatDictionaryNode>(ownerFullPath, propRootName, propFullHash);
        pfdn?.Remove(k);
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropStringDictionaryRemoveRemote(PropStringNode key, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string k = key.GetValue();
        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropStringDictionaryNode? psdn = entity.GetPropNode<PropStringDictionaryNode>(ownerFullPath, propRootName, propFullHash);
        psdn?.Remove(k);
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropIntDictionaryClearRemote(PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropIntDictionaryNode? pidn = entity.GetPropNode<PropIntDictionaryNode>(ownerFullPath, propRootName, propFullHash);
        pidn?.Clear();
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropFloatDictionaryClearRemote(PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropFloatDictionaryNode? pfdn = entity.GetPropNode<PropFloatDictionaryNode>(ownerFullPath, propRootName, propFullHash);
        pfdn?.Clear();
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropStringDictionaryClearRemote(PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropStringDictionaryNode? psdn = entity.GetPropNode<PropStringDictionaryNode>(ownerFullPath, propRootName, propFullHash);
        psdn?.Clear();
    }

    [Rpc(RpcConst.Server, PropNodeConst.TypeVector3, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropVector3SetterRemote(PropVector3Node value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        PropVector3Node? pv3n = entity.GetPropNode<PropVector3Node>(ownerFullPath, propRootName, propFullHash);
        pv3n?.SetValue(value.GetValue());
    }

    #endregion
}
