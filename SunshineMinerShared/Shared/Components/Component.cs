using System.Collections;
using System.Collections.Generic;

public class Component
{
    /*
     * Init properties from dict
     */
    public virtual void InitFromDict(CustomDict property)
    {

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
}
