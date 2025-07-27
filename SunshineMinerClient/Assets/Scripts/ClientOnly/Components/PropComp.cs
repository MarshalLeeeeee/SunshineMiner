using System;
using System.Reflection;

public class PropComp : PropCompCommon
{
    [Rpc(RpcConst.Server, PropNodeConst.DataTypeFloat, PropNodeConst.DataTypeString, PropNodeConst.DataTypeString)]
    public void PropFloatSetterRemote(PropFloatNode value, PropStringNode ownerFullPath_, PropStringNode name_)
    {
        if (entity == null) return;

        string ownerFullPath = ownerFullPath_.GetValue();
        string name = name_.GetValue();
        Component? owner = entity.GetComponentByFullPath(ownerFullPath);
        if (owner == null) return;

        Type type = owner.GetType();
        PropertyInfo? property = type.GetProperty(
            name,
            BindingFlags.Public | BindingFlags.Instance
        );
        if (property != null)
        {
            object p = property.GetValue(owner);
            if (p != null && p is PropFloatNode propNode)
            {
                propNode.SetValue(value.GetValue());
            }
        }
        FieldInfo? field = type.GetField(
            name,
            BindingFlags.Public | BindingFlags.Instance
        );
        if (field != null)
        {
            object f = field.GetValue(owner);
            if (f != null && f is PropFloatNode propNode)
            {
                propNode.SetValue(value.GetValue());
            }
        }
    }
}
