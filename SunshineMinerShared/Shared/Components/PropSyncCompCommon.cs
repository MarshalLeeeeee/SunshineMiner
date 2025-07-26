using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class PropSyncCompCommon : Component
{
    protected override void DoEnableSelf()
    {
        base.DoEnableSelf();
        EnbalePropSync();
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.GetValue(), "EnableComponent", "EnableCompPropSync", EnableCompPropSync);
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.GetValue(), "DisableComponent", "DisableCompPropSync", DisableCompPropSync);
    }

    protected override void DoDisableSelf()
    {
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.GetValue(), "EnableComponent", "EnableCompPropSync");
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.GetValue(), "DisableComponent", "DisableCompPropSync");
        DisablePropSync();
        base.DoDisableSelf();
    }

    protected void EnbalePropSync()
    {
        if (entity == null) return;

        // enable prop sync for entity
        foreach ((PropNode property, int syncType, string propName) in GetSyncProperties(entity))
        {
            EnableCustomTypeSync(property, syncType, "base", propName);
        }
        foreach ((PropNode field, int syncType, string fieldName) in GetSyncFields(entity))
        {
            EnableCustomTypeSync(field, syncType, "base", fieldName);
        }

        // enable prop sync for enabled comps
        foreach (KeyValuePair<string, Component> kvp in entity.IterComponents()) // TODO: nested components
        {
            Component comp = kvp.Value;
            EnableCompPropSync(comp);
        }
    }

    protected void DisablePropSync()
    {
        if (entity == null) return;

        // enable prop sync for entity
        foreach ((PropNode property, int syncType, string propName) in GetSyncProperties(entity))
        {
            DisableCustomTypeSync(property);
        }
        foreach ((PropNode field, int syncType, string fieldName) in GetSyncFields(entity))
        {
            DisableCustomTypeSync(field);
        }

        // enable prop sync for enabled comps
        foreach (KeyValuePair<string, Component> kvp in entity.IterComponents()) // TODO: nested components
        {
            Component comp = kvp.Value;
            DisableCompPropSync(comp);
        }
    }

    protected void EnableCompPropSync(Component comp) 
    {
        Type compType = comp.GetType();
        string compName = compType.Name;
        foreach ((PropNode property, int syncType, string propName) in GetSyncProperties(comp))
        {
            EnableCustomTypeSync(property, syncType, compName, propName);
        }
        foreach ((PropNode field, int syncType, string fieldName) in GetSyncFields(comp))
        {
            EnableCustomTypeSync(field, syncType, compName, fieldName);
        }
    }

    protected void DisableCompPropSync(Component comp)
    {
        foreach ((PropNode property, int syncType, string propName) in GetSyncProperties(comp))
        {
            DisableCustomTypeSync(property);
        }
        foreach ((PropNode field, int syncType, string filedName) in GetSyncFields(comp))
        {
            DisableCustomTypeSync(field);
        }
    }

    private IEnumerable<(PropNode property, int syncType, string propName)> GetSyncProperties(object instance)
    {
        Type type = instance.GetType();
        var propertyInfos = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            PropertySyncAttribute attr = propertyInfo.GetCustomAttribute<PropertySyncAttribute>();
            if (attr != null)
            {
                var value = propertyInfo.GetValue(instance);
                if (value != null && value is PropNode property)
                {
                    yield return (property, attr.syncType, propertyInfo.Name);
                }
            }
        }
    }

    private IEnumerable<(PropNode property, int syncType, string fieldName)> GetSyncFields(object instance)
    {
        Type type = instance.GetType();
        var fieldInfos = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            PropertySyncAttribute attr = fieldInfo.GetCustomAttribute<PropertySyncAttribute>();
            if (attr != null)
            {
                var value = fieldInfo.GetValue(instance);
                if (value != null && value is PropNode field)
                {
                    yield return (field, attr.syncType, fieldInfo.Name);
                }
            }
        }
    }

    private void EnableCustomTypeSync(PropNode arg, int syncType, string objectName, string name)
    {
        if (arg is PropIntNode intArg)
        {
            intArg.OnSetter = GetOnSetter(syncType, objectName, name);
        }
        else if (arg is PropFloatNode floatArg)
        {
            floatArg.OnSetter = GetOnSetter(syncType, objectName, name);
        }
    }

    private void DisableCustomTypeSync(PropNode arg)
    {
        if (arg is PropIntNode intArg)
        {
            intArg.OnSetter = null;
        }
        else if (arg is PropFloatNode floatArg)
        {
            floatArg.OnSetter = null;
        }
    }

    protected virtual Action<PropNode, PropNode>? GetOnSetter(int syncType, string objectName, string name)
    {
        return null;
    }
}
