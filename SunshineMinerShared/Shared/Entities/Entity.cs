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

    /*
     * Init components
     */
    public void Init()
    {
        InitComponents();
    }

    /*
     * Sync base property
     * Init components
     * Sync component property
     */
    public void Init(CustomDict baseProperty, CustomDict compProperty)
    {
        Type type = GetType();
        foreach (DictionaryEntry kvp in baseProperty)
        {
            string name = ((CustomString)(kvp.Key)).Getter();
            PropertyInfo? property = type.GetProperty(
                name,
                BindingFlags.Public | BindingFlags.Instance
            );
            if (property != null)
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
        InitComponents();
        foreach (DictionaryEntry kvp in compProperty)
        {
            string compName = ((CustomString)(kvp.Key)).Getter();
            Component? component = GetComponentByName(compName);
            if (component != null)
            {
                component.Init(this, (CustomDict)(kvp.Value));
            }
        }
    }

    public virtual void Enable()
    {
        foreach (Component component in components.Values)
        {
            component.Enable();
        }
    }

    public virtual void Update()
    {
        foreach (Component component in components.Values)
        {
            component.Update();
        }
    }

    public virtual void Disable()
    {
        foreach (Component component in components.Values)
        {
            component.Disable();
        }
    }

    public virtual void Destroy()
    {
        foreach (Component component in components.Values)
        {
            component.Destroy();
        }
        components.Clear();
    }

    #region REGION_COMPONENT_MANAGEMENT

    public T? GetComponent<T>() where T : Component
    {
        Type type = typeof(T);
        string compName = type.Name;
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

    public Component? GetComponentByName(string compName)
    {
        if (components.TryGetValue(compName, out Component component))
        {
            if (component != null)
            {
                return component;
            }
            else return null;
        }
        else return null;
    }

    public IEnumerable<KeyValuePair<string, Component>> IterComponents()
    {
        foreach (KeyValuePair<string, Component> kvp in components)
        {
            yield return kvp;
        }
    }

    protected virtual void InitComponents() { }

    public T InitComponent<T>() where T : Component, new()
    {
        Type type = typeof(T);
        string compName = type.Name;
        if (!components.ContainsKey(compName))
        {
            components[compName] = new T();
            components[compName].Init(this);
        }
        return (T)components[compName];
    }

    public T InitComponent<T>(CustomDict compProperty) where T : Component, new()
    {
        Type type = typeof(T);
        string compName = type.Name;
        if (!components.ContainsKey(compName))
        {
            components[compName] = new T();
            components[compName].Init(this, compProperty);
        }
        return (T)components[compName];
    }

    public void EnableComponent<T>() where T : Component
    {
        Type type = typeof(T);
        string compName = type.Name;
        if (components.TryGetValue(compName, out Component? component))
        {
            if (component != null && component is T comp)
            {
                comp.Enable();
            }
        }
    }

    public void EnableComponentByName(string compName)
    {
        if (components.TryGetValue(compName, out Component? component))
        {
            if (component != null)
            {
                component.Enable();
            }
        }
    }

    public void DisableComponent<T>() where T : Component
    {
        Type type = typeof(T);
        string compName = type.Name;
        if (components.TryGetValue(compName, out Component? component))
        {
            if (component != null && component is T comp)
            {
                comp.Disable();
            }
        }
    }

    public void DisableComponentByName(string compName)
    {
        if (components.TryGetValue(compName, out Component? component))
        {
            if (component != null)
            {
                component.Disable();
            }
        }
    }

    public void DestroyComponent<T>() where T : Component
    {
        Type type = typeof(T);
        string compName = type.Name;
        if (components.TryGetValue(compName, out Component? component))
        {
            if (component != null && component is T comp)
            {
                comp.Disable();
            }
        }
    }

    public void DestroyComponentByName(string compName)
    {
        if (components.TryGetValue(compName, out Component? component))
        {
            if (component != null)
            {
                component.Disable();
            }
        }
        components.Remove(compName);
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
        return properties;
    }

    #endregion
}
