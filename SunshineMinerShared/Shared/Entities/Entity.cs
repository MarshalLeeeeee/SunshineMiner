using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using Microsoft.VisualBasic;


public class Entity
{
    [PropertySync(SyncConst.AllClient)]
    public CustomString eid = new CustomString();

    private Dictionary<string, Component> components = new Dictionary<string, Component>();

    private Dictionary<string, RpcMethodInfo> rpcMethods = new Dictionary<string, RpcMethodInfo>();

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
        InitRpcMethods();
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
        InitRpcMethods();
        InitComponents();
        foreach (DictionaryEntry kvp in compProperty)
        {
            string compName = ((CustomString)(kvp.Key)).Getter();
            Component? component = GetComponent<Component>(compName);
            if (component != null)
            {
                component.Init(this, (CustomDict)(kvp.Value));
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
        PostUpdate();
    }

    protected virtual void PostUpdate()
    {
        
    }

    public virtual void OnUnload()
    {
        foreach (Component component in components.Values)
        {
            component.OnUnload();
        }
    }

    #region REGION_COMPONENT_MANAGEMENT

    protected virtual void InitComponents() { }

    public T InitComponent<T>(string compName) where T : Component, new()
    {
        if (!components.ContainsKey(compName))
        {
            components[compName] = new T();
            components[compName].Init(this);
        }
        return (T)components[compName];
    }

    public T InitComponent<T>(string compName, CustomDict compProperty) where T : Component, new()
    {
        if (!components.ContainsKey(compName))
        {
            components[compName] = new T();
            components[compName].Init(this, compProperty);
        }
        return (T)components[compName];
    }

    public void LoadComponent<T>(string compName) where T : Component, new()
    {
        T component = InitComponent<T>(compName);
        component.OnLoad();
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
        return properties;
    }

    #endregion

    #region REGION_RPC

    public void InitRpcMethods()
    {
        Type type = GetType();
        var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (MethodInfo method in methods)
        {
            var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
            if (rpcAttr == null)
            {
                continue;
            }
            rpcMethods[method.Name] = new RpcMethodInfo(method);
        }
    }

    public void InitComponentRpcMethods(string compName)
    {
        Component? comp = GetComponent<Component>(compName);
        if (comp == null) return;

        Type type = comp.GetType();
        var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (MethodInfo method in methods)
        {
            var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
            if (rpcAttr == null)
            {
                continue;
            }
            rpcMethods[method.Name] = new RpcMethodInfo(compName, method);
        }
    }

    public RpcMethodInfo? GetRpcMethodInfo(string methodName)
    {
        if (rpcMethods.TryGetValue(methodName, out RpcMethodInfo? rpcMethod))
        {
            return rpcMethod;
        }
        else
        {
            return null;
        }
    }

    #endregion
}
