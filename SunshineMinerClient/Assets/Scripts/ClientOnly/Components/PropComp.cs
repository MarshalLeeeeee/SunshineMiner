using System;
using System.Reflection;

[Comp]
public class PropComp : PropCompCommon
{
    [Rpc(RpcConst.Server, PropNodeConst.TypeFloat, PropNodeConst.TypeString, PropNodeConst.TypeString, PropNodeConst.TypeString)]
    public void PropFloatSetterRemote(PropFloatNode value, PropStringNode ownerFullPath_, PropStringNode propRootName_, PropStringNode propFullHash_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string propRootName = propRootName_.GetValue();
        string propFullHash = propFullHash_.GetValue();
        Component? owner = entity.GetComponentByFullPath(ownerFullPath);
        if (owner == null) return;

        Type type = owner.GetType();
        PropertyInfo? property = type.GetProperty(
            propRootName,
            BindingFlags.Public | BindingFlags.Instance
        );
        if (property != null)
        {
            object p = property.GetValue(owner);
            if (p != null && p is PropNode propNode)
            {
                PropNode? pp = propNode.GetNodeByHash(propFullHash);
                if (pp != null && pp is PropFloatNode ppp)
                {
                    ppp.SetValue(value.GetValue());
                }
            }
        }
        FieldInfo? field = type.GetField(
            propRootName,
            BindingFlags.Public | BindingFlags.Instance
        );
        if (field != null)
        {
            object f = field.GetValue(owner);
            if (f != null && f is PropFloatNode propNode)
            {
                PropNode? ff = propNode.GetNodeByHash(propFullHash);
                if (ff != null && ff is PropFloatNode fff)
                {
                    fff.SetValue(value.GetValue());
                }
            }
        }
    }
}
