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
        Debugger.Log($"Creating manager: {name}");
        managers[name] = mgr;
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

    public void StartManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Start();
        }
    }

    public void UpdateManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Update();
        }
    }

    public void StopManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Stop();
        }
        managers.Clear();
    }
}
