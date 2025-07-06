using System;
using System.Reflection;
using System.Xml.Linq;

public class PropSyncComp : PropSyncCompCommon
{
    protected override Action<CustomType, CustomType>? GetOnSetter(int syncType, string objectName, string name)
    {
        return OnSetter;
    }

    private void OnSetter(CustomType o, CustomType n)
    {
        Debugger.Log($"PropSyncComp OnSetter old {o.CustomToString()} new {n.CustomToString()}");
    }

    [Rpc(RpcConst.Server, CustomTypeConst.TypeUndefined, CustomTypeConst.TypeString, CustomTypeConst.TypeString)]
    public void PropSyncSetterRemote(CustomType value, CustomString objectName_, CustomString name_)
    {
        string objectName = objectName_.Getter();
        string name = name_.Getter();
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
