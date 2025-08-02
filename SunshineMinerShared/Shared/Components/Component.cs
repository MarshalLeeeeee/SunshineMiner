using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/*
* Component is a func node that can be attached to an entity and has sub components.
* It provides methods for initialization, enabling, updating, disabling, and destroying components.
*/
public class Component : FuncNode
{
    protected Entity? _entity = null; // The root of this func node tree, which is an Entity.
    public Entity? entity
    {
        get 
        { 
            if (parent is not null) return _entity;
            else
            {
                if (this is Entity e) return e;
                else return null;
            }
        }
    }
    public bool enabled { get; protected set; } = false;

    public override void SetParent(FuncNode? p)
    {
        base.SetParent(p);
        if (p is not null && p is Component component)
        {
            _entity = component.entity;
        }
    }

    #region REGION_BEHAVIOR

    public void Init()
    {
        InitComponents();
    }

    public void Init(PropDictionaryNode<string> info)
    {
        Type type = GetType();
        if (info.TryGetValue("_Property", out PropNode? dataNode) && dataNode is PropDictionaryNode<string> propertyInfo)
        {
            foreach (KeyValuePair<string, PropNode> kvp in propertyInfo)
            {
                string name = kvp.Key;
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
        }
        InitComponents();
        foreach (KeyValuePair<string, PropNode> kvp in info)
        {
            string name = kvp.Key;
            if (name == "_Property") continue;
            if (kvp.Value is PropDictionaryNode<string> compInfo)
            {
                InitComponentByName(name, compInfo);
            }
        }
    }

    /*
     * invoked when enabled
     */
    public void Enable()
    {
        if (enabled) return;
        DoEnableSelf();
        foreach (FuncNode funcNode in funcNodes.Values)
        {
            if (funcNode is Component component)
            {
                component.Enable();
            }
        }
        enabled = true;
        OnEnabled();
    }

    /*
     * invoked when the component is enabled
     * override this method to implement custom enable logic
     */
    protected virtual void DoEnableSelf()
    {

    }

    /*
     * invoked when the component is enabled
     * this method is called after DoEnableSelf()
     */
    protected virtual void OnEnabled()
    {
        Game.Instance.eventManager.TriggerEntityEvent(entity.eid.GetValue(), "EnableComponent", this);
    }

    /*
     * update in game tick
     */
    public void Update ()
    {
        if (!enabled) return;
        DoUpdateSelf();
        foreach (FuncNode funcNode in funcNodes.Values)
        {
            if (funcNode is Component component)
            {
                component.Update();
            }
        }
    }

    /*
     * update in game tick
     * override this method to implement custom update logic
     */
    protected virtual void DoUpdateSelf()
    {

    }

    /*
     * invoked when the component is disabled
     */
    public void Disable()
    {
        if (!enabled) return;
        DoDisableSelf();
        foreach (FuncNode funcNode in funcNodes.Values)
        {
            if (funcNode is Component component)
            {
                component.Disable();
            }
        }
        enabled = false;
        OnDisabled();
    }

    /*
     * invoked when the component is disabled
     * override this method to implement custom disable logic
     */
    protected virtual void DoDisableSelf()
    {

    }

    /*
     * invoked when the component is disabled
     * this method is called after DoDisableSelf()
     */
    protected virtual void OnDisabled()
    {
        Game.Instance.eventManager.TriggerEntityEvent(entity.eid.GetValue(), "DisableComponent", this);
    }

    /*
     * invoked when the component is unloaded from an entity
     * clear the entity reference
     * ready to be destroyed and recycled
     */
    public virtual void Destroy()
    {
        foreach (FuncNode funcNode in funcNodes.Values)
        {
            if (funcNode is Component component)
            {
                component.Destroy();
            }
        }
        funcNodes.Clear();
    }

    #endregion

    #region REGION_COMPONENT_MANAGEMENT

    public T? GetComponent<T>() where T : Component
    {
        T? funcNode = GetFuncNode<T>();
        if (funcNode != null)
        {
            return funcNode;
        }
        else return null;
    }

    public Component? GetComponentByName(string name)
    {
        FuncNode? funcNode = GetFuncNodeByName(name);
        if (funcNode != null && funcNode is Component component)
        {
            return component;
        }
        else return null;
    }

    public Component? GetComponentByFullPath(string fullPath)
    {
        FuncNode? funcNode = GetFuncNodeByFullPath(fullPath);
        if (funcNode != null && funcNode is Component component)
        {
            return component;
        }
        else return null;
    }

    public IEnumerable<KeyValuePair<string, Component>> IterComponents()
    {
        foreach (KeyValuePair<string, FuncNode> kvp in funcNodes)
        {
            if (kvp.Value is Component component)
            {
                yield return new KeyValuePair<string, Component>(kvp.Key, component);
            }
        }
    }

    protected virtual void InitComponents() { }

    public T InitComponent<T>() where T : Component, new()
    {
        Type type = typeof(T);
        string compName = type.Name;
        T? component = GetComponent<T>();
        if (component == null)
        {
            component = AddFuncNode<T>(this);
            component.Init();
        }
        return component;
    }

    public T InitComponent<T>(PropDictionaryNode<string> info) where T : Component, new()
    {
        Type type = typeof(T);
        string compName = type.Name;
        T? component = GetComponent<T>();
        if (component == null)
        {
            component = AddFuncNode<T>(this);
            component.Init(info);
        }
        return component;
    }

    public void InitComponentByName(string compName)
    {
        Component? component = GetComponentByName(compName);
        if (component == null)
        {
            component = Factory.CreateComponent(compName);
            if (component == null) return;
            AddFuncNode(component, this);
            component.Init();
        }
    }

    public void InitComponentByName(string compName, PropDictionaryNode<string> info)
    {
        Component? component = GetComponentByName(compName);
        if (component == null)
        {
            component = Factory.CreateComponent(compName);
            if (component == null) return;
            AddFuncNode(component, this);
            component.Init(info);
        }
    }

    public void EnableComponent<T>() where T : Component
    {
        T? component = GetComponent<T>();
        if (component != null)
        {
            component.Enable();
        }
    }

    public void EnableComponentByName(string compName)
    {
        FuncNode? funcNode = GetFuncNodeByName(compName);
        if (funcNode != null && funcNode is Component component)
        {
            component.Enable();
        }
    }

    public void DisableComponent<T>() where T : Component
    {
        T? component = GetComponent<T>();
        if (component != null)
        {
            component.Disable();
        }
    }

    public void DisableComponentByName(string compName)
    {
        FuncNode? funcNode = GetFuncNodeByName(compName);
        if (funcNode != null && funcNode is Component component)
        {
            component.Disable();
        }
    }

    public void DestroyComponent<T>() where T : Component
    {
        T? component = GetComponent<T>();
        if (component != null)
        {
            component.Disable();
            RemoveFuncNode<T>();
        }
    }

    public void DestroyComponentByName(string compName)
    {
        FuncNode? funcNode = GetFuncNodeByName(compName);
        if (funcNode != null && funcNode is Component component)
        {
            component.Disable();
            RemoveFuncNodeByName(compName);
        }
    }

    protected T? GetEntityComponent<T>() where T : Component
    {
        if (entity != null)
        {
            return entity.GetComponent<T>();
        }
        else if (this is Entity entity)
        {
            return GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    #endregion

    #region REGION_SERIALIZE

    public PropDictionaryNode<string> SerializeWithSyncType(int syncType)
    {
        PropDictionaryNode<string> info = new PropDictionaryNode<string>();
        PropDictionaryNode<string> propInfo = PropStreamer.SerializeInstance(this, syncType);
        if (propInfo.Count > 0)
        {
            info.Add("_Property", propInfo);
        }
        foreach (var kvp in IterComponents())
        {
            PropDictionaryNode<string> compInfo = kvp.Value.SerializeWithSyncType(syncType);
            if (compInfo.Count > 0)
            {
                info.Add(kvp.Key, compInfo);
            }
        }
        return info;
    }

    #endregion

}

