using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/*
* Entity is a base class for all entities in the game.
* Entity is a special kind of Component. 
* It always acts as a root node of a components tree.
* It provides methods for initialization, enabling, updating, disabling, and destroying components.
*/
public class Entity : Component
{
    [PropertySync(SyncConst.AllClient)]
    public PropStringNode eid = new PropStringNode();

    public Entity() { }

    public Entity(string eid_)
    {
        eid = new PropStringNode(eid_);
    }

    /*
     * invoked when the component is enabled
     * this method is called after DoEnableSelf()
     */
    protected override void OnEnabled()
    {
        Game.Instance.eventManager.TriggerEntityEvent(entity.eid.GetValue(), "EnableEntity", this);
    }

    /*
     * invoked when the component is disabled
     * this method is called after DoDisableSelf()
     */
    protected override void OnDisabled()
    {
        Game.Instance.eventManager.TriggerEntityEvent(entity.eid.GetValue(), "DisableEntity", this);
    }
}
