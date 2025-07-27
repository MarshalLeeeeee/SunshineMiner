using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class PropCompCommon : Component
{
    HashSet<string> enabledComponents = new HashSet<string>();

    protected override void DoEnableSelf()
    {
        base.DoEnableSelf();
        EnableProp();
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.GetValue(), "EnableComponent", "DoEnablePropRecursive", DoEnablePropRecursive);
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.GetValue(), "DisableComponent", "DoDisablePropRecursive", DoDisablePropRecursive);
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.GetValue(), "EnableEntity", "DoEnablePropRecursive", DoEnablePropRecursive);
        Game.Instance.eventManager.RegisterEntityEvent<Component>(entity.eid.GetValue(), "DisableableEntity", "DoDisablePropRecursive", DoDisablePropRecursive);
    }

    protected override void DoDisableSelf()
    {
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.GetValue(), "EnableComponent", "DoEnablePropRecursive");
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.GetValue(), "DisableComponent", "DoDisablePropRecursive");
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.GetValue(), "EnableEntity", "DoEnablePropRecursive");
        Game.Instance.eventManager.UnregisterEntityEvent(entity.eid.GetValue(), "DisableableEntity", "DoDisablePropRecursive");
        DisableProp();
        base.DoDisableSelf();
    }

    protected void EnableProp()
    {
        if (entity == null) return;
        DoEnableProp();
    }

    protected void DoEnableProp()
    {
        DoEnablePropRecursive(entity);
    }

    protected void DoEnablePropRecursive(Component node)
    {
        if (!node.enabled) return;
        DoEnablePropWithNode(node);
        foreach (KeyValuePair<string, Component> kvp in node.IterComponents())
        {
            Component comp = kvp.Value;
            DoEnablePropRecursive(comp);
        }
    }

    private void DoEnablePropWithNode(Component node)
    {
        Type type = node.GetType();
        var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            PropertySyncAttribute attr = propertyInfo.GetCustomAttribute<PropertySyncAttribute>();
            if (attr != null)
            {
                var value = propertyInfo.GetValue(node);
                if (value != null && value is PropNode prop)
                {
                    prop.owner = node;
                    prop.name = propertyInfo.Name;
                    prop.syncType = attr.syncType;
                }
            }
        }
        var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            PropertySyncAttribute attr = fieldInfo.GetCustomAttribute<PropertySyncAttribute>();
            if (attr != null)
            {
                var value = fieldInfo.GetValue(node);
                if (value != null && value is PropNode prop)
                {
                    prop.owner = node;
                    prop.name = fieldInfo.Name;
                    prop.syncType = attr.syncType;
                }
            }
        }
    }

    protected void DisableProp()
    {
        if (entity == null) return;
        DoDisableProp();
    }

    protected void DoDisableProp()
    {
        DoDisablePropRecursive(entity);
    }

    protected void DoDisablePropRecursive(Component node)
    {
        DoDisablePropWithNode(node);
        foreach (KeyValuePair<string, Component> kvp in node.IterComponents())
        {
            Component comp = kvp.Value;
            DoDisablePropRecursive(comp);
        }
    }

    private void DoDisablePropWithNode(Component node)
    {
        Type type = node.GetType();
        var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            PropertySyncAttribute attr = propertyInfo.GetCustomAttribute<PropertySyncAttribute>();
            if (attr != null)
            {
                var value = propertyInfo.GetValue(node);
                if (value != null && value is PropNode prop)
                {
                    prop.owner = null;
                    prop.name = "";
                    prop.syncType = SyncConst.Undefined;
                }
            }
        }
        var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            PropertySyncAttribute attr = fieldInfo.GetCustomAttribute<PropertySyncAttribute>();
            if (attr != null)
            {
                var value = fieldInfo.GetValue(node);
                if (value != null && value is PropNode prop)
                {
                    prop.owner = null;
                    prop.name = "";
                    prop.syncType = SyncConst.Undefined;
                }
            }
        }
    }

    public virtual void OnFloatSetter(float o, float n, int syncType, FuncNode? owner, string name) {}
}
