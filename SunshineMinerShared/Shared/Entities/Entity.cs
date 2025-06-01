using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


public class Entity
{
    [PropertySync(SyncConst.AllClient)]
    public CustomString eid = new CustomString();

    private Dictionary<string, Component> components = new Dictionary<string, Component>();

    public Entity() { }

    public Entity(string eid_)
    {
        eid = new CustomString(eid_);
    }

    public virtual void InitFromDict(CustomDict baseProperty, CustomDict compProperty)
    {
        Type type = GetType();
        foreach (DictionaryEntry kvp in baseProperty)
        {
            string name = ((CustomString)(kvp.Key)).Getter();
            PropertyInfo? property = type.GetProperty(
                name,
                BindingFlags.Public | BindingFlags.Instance
            );
            if ( property != null )
            {
                property.SetValue(this, kvp.Value);
            }
            FieldInfo? field = type.GetField(
                name,
                BindingFlags.Public | BindingFlags.Instance
            );
            if (field != null)
            {
                field.SetValue(this, kvp.Value);
            }
        }
    }

    public virtual void OnLoad()
    {
        foreach (Component component in components.Values)
        {
            component.OnLoad();
        }
    }

    public virtual void Update()
    {
        foreach (Component component in components.Values)
        {
            component.Update();
        }
    }

    public virtual void OnUnload()
    {
        foreach (Component component in components.Values)
        {
            component.OnUnload();
        }
    }

    #region REGION_COMPONENT_MANAGEMENT

    public void LoadComponent<T>(string compName) where T : Component, new()
    {
        Debugger.Log($"LoadComponent: {compName}");
        if (components.ContainsKey(compName))
        {
            return;
        }
        components[compName] = new T();
        components[compName].OnLoad();
    }

    public T? GetComponent<T>(string compName) where T : Component
    {
        if (components.TryGetValue(compName, out var component))
        {
            if (component != null && component is T comp)
            {
                return comp;
            }
            else return null;
        }
        else return null;
    }

    public void UnloadComponent(string compName)
    {
        if (components.Remove(compName, out Component? component))
        {
            if (component != null)
            {
                component.OnUnload();
            }
        }
    }

    #endregion

    #region REGION_SERIALIZE

    public CustomList SerializeProperty(int syncType)
    {
        CustomList properties = new CustomList();
        CustomDict baseProperty = CustomTypeStreamer.SerializeProperties(this, syncType);
        CustomDict compProperty = new CustomDict();
        foreach (var kvp in components)
        {
            compProperty.Add(new CustomString(kvp.Key), CustomTypeStreamer.SerializeProperties(kvp.Value, syncType));
        }
        properties.Add(baseProperty);
        properties.Add(compProperty);
        Debugger.Log($"{properties.CustomToString()}");
        return properties;
    }

    #endregion
}
