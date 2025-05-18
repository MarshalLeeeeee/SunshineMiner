using System;
using System.Collections;
using System.Collections.Generic;

public class EntityManager : Manager
{
    private Dictionary<string, PlayerEntity> players = new Dictionary<string, PlayerEntity>();
    private Dictionary<string, Manager> managers = new Dictionary<string, Manager>();

    public EntityManager(string eid) : base(eid) { }

    #region REGION_CREATE_ENTITY

    public T CreateEntity<T>() where T : Entity
    {
        Guid eid = Guid.NewGuid();
        return CreateEntity<T>(eid.ToString());
    }
    public T CreateEntity<T>(string eid) where T : Entity
    {
        return DoCreateEntity<T>(eid);
    }

    public T CreateManager<T>(string mgrName) where T : Manager
    {
        T manager = CreateEntity<T>();
        managers[mgrName] = manager;
        return manager;
    }

    private T DoCreateEntity<T>(string eid) where T : Entity
    {
        T entity = (T)Activator.CreateInstance(typeof(T), eid);
        return entity;
    }

    #endregion

    #region REGION_GET_ENITTY

    public Entity GetEntity(string s)
    {
        if (s == "EntityManager")
        {
            return this;
        }
        else if (managers.TryGetValue(s, out var manager))
        {
            return manager;
        }
        else
        {
            return null;
        }
    }

    #endregion
}
