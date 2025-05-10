using System;
using System.Collections;
using System.Collections.Generic;

public class Entity
{
    [SyncType(SyncConst.AllClient)]
    public CustomString eid { get; private set; }

    private Dictionary<string, Component> components = new Dictionary<string, Component>();

    public Entity(string eid_)
    {
        eid = new CustomString(eid_);
    }

    #region REGION_COMPONENT_MANAGEMENT

    public void LoadComponent<T>(string compName) where T : Component, new()
    {
        if (components.ContainsKey(compName))
        {
            return;
        }
        components[compName] = new T();
        components[compName].OnLoad();
    }

    public T GetComponent<T>(string compName) where T : Component
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
        if (components.Remove(compName, out Component component))
        {
            if (component != null)
            {
                component.OnUnload();
            }
        }
    }

    #endregion

    #region REGION_PROPERTY_SERIALIZATION



    #endregion
}
