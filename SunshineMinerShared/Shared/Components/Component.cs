using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Component
{
    protected Entity? entity = null;
    protected bool enabled = false;

    /*
     * invoked when the componented is loaded to an entity
     * Set corresponding entity eid
     */
    public virtual void Init(Entity e)
    {
        entity = e;
    }

    /*
     * invoked when the componented is loaded to an entity
     * set corresponding entity eid
     * init properties from dict
     */
    public virtual void Init(Entity e, DataDictionaryNode<string> compProperty)
    {
        entity = e;
        Type type = GetType();
        foreach (KeyValuePair<string, DataNode> kvp in compProperty)
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

    /*
     * invoked when the component is enabled
     */
    public void Enable()
    {
        if (enabled) return;
        DoEnable();
        enabled = true;
        Game.Instance.eventManager.TriggerEntityEvent(entity.eid.GetValue(), "EnableComponent", this);
    }

    /*
     * invoked when the component is enabled
     * override this method to implement custom enable logic
     */
    protected virtual void DoEnable()
    {

    }

    /*
     * update in game tick
     */
    public virtual void Update ()
    {

    }

    /*
     * invoked when the component is disabled
     */
    public void Disable()
    {
        if (!enabled) return;
        DoDisable();
        enabled = false;
        Game.Instance.eventManager.TriggerEntityEvent(entity.eid.GetValue(), "DisableComponent", this);
    }

    /*
     * invoked when the component is disabled
     * override this method to implement custom disable logic
     */
    protected virtual void DoDisable()
    {

    }

    /*
     * invoked when the component is unloaded from an entity
     * clear the entity reference
     * ready to be destroyed and recycled
     */
    public virtual void Destroy()
    {
        entity = null;
    }

    protected T? GetComponent<T>() where T : Component
    {
        if (entity != null)
        {
            return entity.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }
}

