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
    }

    protected override void DoDisableSelf()
    {
        DisableProp();
        base.DoDisableSelf();
    }

    protected void EnableProp()
    {
        if (entity != null)
        {
            DoEnablePropRecursive(entity);
            string eid = entity.eid.GetValue();
            Game.Instance.eventManager.RegisterEntityEvent<Component>(eid, "EnableComponent", "DoEnableProp", DoEnableProp);
            Game.Instance.eventManager.RegisterEntityEvent<Component>(eid, "DisableComponent", "DoDisableProp", DoDisableProp);
            Game.Instance.eventManager.RegisterEntityEvent<Component>(eid, "EnableEntity", "DoEnableProp", DoEnableProp);
            Game.Instance.eventManager.RegisterEntityEvent<Component>(eid, "DisableEntity", "DoDisableProp", DoDisableProp);
        }
    }

    /*
    * Enable prop the subtree of the given component
    */
    protected void DoEnablePropRecursive(Component node)
    {
        if (node == null) return;
        if (!node.enabled) return;
        DoEnableProp(node);
        foreach (KeyValuePair<string, Component> kvp in node.IterComponents())
        {
            Component comp = kvp.Value;
            DoEnablePropRecursive(comp);
        }
    }

    /*
    * Enable prop of the given component
    */
    protected void DoEnableProp(Component node)
    {
        if (node == null) return;
        if (!node.enabled) return;
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
        if (entity != null)
        {
            string eid = entity.eid.GetValue();
            Game.Instance.eventManager.UnregisterEntityEvent(eid, "EnableComponent", "DoEnableProp");
            Game.Instance.eventManager.UnregisterEntityEvent(eid, "DisableComponent", "DoDisableProp");
            Game.Instance.eventManager.UnregisterEntityEvent(eid, "EnableEntity", "DoEnableProp");
            Game.Instance.eventManager.UnregisterEntityEvent(eid, "DisableEntity", "DoDisableProp");
            DoDisablePropRecursive(entity);
        }
    }

    /* 
    * Disable prop the subtree of the given component
    */
    protected void DoDisablePropRecursive(Component node)
    {
        if (node == null) return;
        if (node.enabled) return;
        DoDisableProp(node);
        foreach (KeyValuePair<string, Component> kvp in node.IterComponents())
        {
            Component comp = kvp.Value;
            DoDisablePropRecursive(comp);
        }
    }

    /*
    * Disable prop of the given component
    */
    protected void DoDisableProp(Component node)
    {
        if (node == null) return;
        if (node.enabled) return;
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

    #region REGION_PROP_SET_CALLBACK

    public virtual void OnFloatSetter(float o, float n, int syncType, FuncNode? owner, string name) {}

    #endregion
}
