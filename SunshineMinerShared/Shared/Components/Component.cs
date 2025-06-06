using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Component
{
    protected Entity? entity = null;

    /*
     * Set corresponding entity eid
     */
    public virtual void Init(Entity e)
    {
        entity = e;
        InitRpcMethods();
    }

    /*
     * Set corresponding entity eid
     * Init properties from dict
     */
    public virtual void Init(Entity e, CustomDict compProperty)
    {
        entity = e;
        Type type = GetType();
        foreach (DictionaryEntry kvp in compProperty)
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
    }

    private void InitRpcMethods()
    {
        if (entity == null) return;

        Type type = GetType();
        entity.InitComponentRpcMethods(type.Name);
    }

    /*
     * invoked when the component is loaded to the entity
     */
    public virtual void OnLoad()
    {

    }

    /*
     * update in game tick
     */
    public virtual void Update ()
    {

    }

    /*
     * invoked when the component is unloaded from the entity
     */
    public virtual void OnUnload()
    {
        entity = null;
    }

    protected T? GetComponent<T>(string compName) where T : Component
    {
        if (entity != null)
        {
            return entity.GetComponent<T>(compName);
        }
        else
        {
            return null;
        }
    }
}

