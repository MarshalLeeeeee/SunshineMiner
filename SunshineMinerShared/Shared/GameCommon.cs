using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class GameCommon
{
    protected Dictionary<string, Manager> managers = new Dictionary<string, Manager>();

    protected void CreateManager<T>() where T : Manager, new()
    {
        Type type = typeof(T);
        string name = type.Name;
        T mgr = new T();
        managers[name] = mgr;
        mgr.Init();
    }

    protected T? GetManager<T>() where T : Manager
    {
        Type type = typeof(T);
        string name = type.Name;
        if (managers.TryGetValue(name, out Manager manager))
        {
            if (manager != null && manager is T mgr)
            {
                return mgr;
            }
            return null;
        }
        return null;
    }

    protected virtual void InitManagers() { }

    public void EnableManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Enable();
        }
    }

    public void UpdateManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Update();
        }
    }

    public void DisableManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Disable();
        }
        managers.Clear();
    }
}
