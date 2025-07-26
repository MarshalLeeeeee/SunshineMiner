using System;
using System.Reflection;
using System.Xml.Linq;

public class PropSyncComp : PropSyncCompCommon
{
    protected override Action<PropNode, PropNode>? GetOnSetter(int syncType, string objectName, string name)
    {
        return OnSetter;
    }

    private void OnSetter(PropNode o, PropNode n)
    {
        // Debugger.Log($"PropSyncComp OnSetter old {o} new {n}");
    }

    [Rpc(RpcConst.Server, PropNodeConst.DataTypeUndefined, PropNodeConst.DataTypeString, PropNodeConst.DataTypeString)]
    public void PropSyncSetterRemote(PropNode value, PropStringNode objectName_, PropStringNode name_)
    {
        string objectName = objectName_.GetValue();
        string name = name_.GetValue();
        object? instance = null;
        if (objectName == "base")
        {
            instance = entity;
        }
        else
        {
            instance = entity.GetComponentByName(objectName);
        }
        if (instance == null) return;

        Type type = instance.GetType();
        PropertyInfo? property = type.GetProperty(
            name,
            BindingFlags.Public | BindingFlags.Instance
        );
        if (property != null)
        {
            property.SetValue(instance, value);
        }
        FieldInfo? field = type.GetField(
            name,
            BindingFlags.Public | BindingFlags.Instance
        );
        if (field != null)
        {
            field.SetValue(instance, value);
        }
    }
}
