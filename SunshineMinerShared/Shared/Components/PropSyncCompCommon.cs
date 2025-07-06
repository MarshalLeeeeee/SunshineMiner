using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class PropSyncCompCommon : Component
{
    protected override void DoEnable()
    {
        base.DoEnable();
        EnbalePropSync();
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.Getter(), "EnableComponent", "EnableCompPropSync", EnableCompPropSync);
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.Getter(), "DisableComponent", "DisableCompPropSync", DisableCompPropSync);
    }

    protected override void DoDisable()
    {
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.Getter(), "EnableComponent", "EnableCompPropSync");
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.Getter(), "DisableComponent", "DisableCompPropSync");
        DisablePropSync();
        base.DoDisable();
    }

    protected void EnbalePropSync()
    {
        if (entity == null) return;

        // enable prop sync for entity
        foreach ((CustomType property, int syncType, string propName) in GetSyncProperties(entity))
        {
            EnableCustomTypeSync(property, syncType, "base", propName);
        }
        foreach ((CustomType field, int syncType, string fieldName) in GetSyncFields(entity))
        {
            EnableCustomTypeSync(field, syncType, "base", fieldName);
        }

        // enable prop sync for enabled comps
        foreach (KeyValuePair<string, Component> kvp in entity.IterComponents())
        {
            Component comp = kvp.Value;
            EnableCompPropSync(comp);
        }
    }

    protected void DisablePropSync()
    {
        if (entity == null) return;

        // enable prop sync for entity
        foreach ((CustomType property, int syncType, string propName) in GetSyncProperties(entity))
        {
            DisableCustomTypeSync(property);
        }
        foreach ((CustomType field, int syncType, string fieldName) in GetSyncFields(entity))
        {
            DisableCustomTypeSync(field);
        }

        // enable prop sync for enabled comps
        foreach (KeyValuePair<string, Component> kvp in entity.IterComponents())
        {
            Component comp = kvp.Value;
            DisableCompPropSync(comp);
        }
    }

    protected void EnableCompPropSync(Component comp) 
    {
        Type compType = comp.GetType();
        string compName = compType.Name;
        foreach ((CustomType property, int syncType, string propName) in GetSyncProperties(comp))
        {
            EnableCustomTypeSync(property, syncType, compName, propName);
        }
        foreach ((CustomType field, int syncType, string fieldName) in GetSyncFields(comp))
        {
            EnableCustomTypeSync(field, syncType, compName, fieldName);
        }
    }

    protected void DisableCompPropSync(Component comp)
    {
        foreach ((CustomType property, int syncType, string propName) in GetSyncProperties(comp))
        {
            DisableCustomTypeSync(property);
        }
        foreach ((CustomType field, int syncType, string filedName) in GetSyncFields(comp))
        {
            DisableCustomTypeSync(field);
        }
    }

    private IEnumerable<(CustomType property, int syncType, string propName)> GetSyncProperties(object instance)
    {
        Type type = instance.GetType();
        var propertyInfos = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            PropertySyncAttribute attr = propertyInfo.GetCustomAttribute<PropertySyncAttribute>();
            if (attr != null)
            {
                var value = propertyInfo.GetValue(instance);
                if (value != null && value is CustomType property)
                {
                    yield return (property, attr.syncType, propertyInfo.Name);
                }
            }
        }
    }

    private IEnumerable<(CustomType property, int syncType, string fieldName)> GetSyncFields(object instance)
    {
        Type type = instance.GetType();
        var fieldInfos = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            PropertySyncAttribute attr = fieldInfo.GetCustomAttribute<PropertySyncAttribute>();
            if (attr != null)
            {
                var value = fieldInfo.GetValue(instance);
                if (value != null && value is CustomType field)
                {
                    yield return (field, attr.syncType, fieldInfo.Name);
                }
            }
        }
    }

    private void EnableCustomTypeSync(CustomType arg, int syncType, string objectName, string name)
    {
        int argType = arg.type;
        if (argType == CustomTypeConst.TypeInt)
        {
            ((CustomInt)arg).OnSetter = GetOnSetter(syncType, objectName, name);
        }
        else if (argType == CustomTypeConst.TypeFloat)
        {
            ((CustomFloat)arg).OnSetter = GetOnSetter(syncType, objectName, name);
        }
    }

    private void DisableCustomTypeSync(CustomType arg)
    {
        int argType = arg.type;
        if (argType == CustomTypeConst.TypeInt)
        {
            ((CustomInt)arg).OnSetter = null;
        }
        else if (argType == CustomTypeConst.TypeFloat)
        {
            ((CustomFloat)arg).OnSetter = null;
        }
    }

    protected virtual Action<CustomType, CustomType>? GetOnSetter(int syncType, string objectName, string name)
    {
        return null;
    }
}
