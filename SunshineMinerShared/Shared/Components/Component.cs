using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Component
{
    protected string eid = "";

    protected Entity? entity
    {
        get
        {
            return Game.Instance.entityManager.GetEntity(eid);
        }
    }

    public virtual void Init(string eid)
    {
        this.eid = eid;
    }

    /*
     * Init properties from dict
     */
    public virtual void Init(string eid, CustomDict compProperty)
    {
        Init(eid);
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

    }

    protected T? GetComponent<T>(string compName) where T : Component
    {
        Entity? entity = this.entity;
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

