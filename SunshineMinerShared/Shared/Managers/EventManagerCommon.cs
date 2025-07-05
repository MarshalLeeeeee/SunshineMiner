using System;
using System.Collections.Generic;

public class EventManagerCommon : Manager
{
    private Dictionary<string, Dictionary<string, Delegate>> globalEvents = new Dictionary<string, Dictionary<string, Delegate>>();
    private Dictionary<string, Dictionary<string, Dictionary<string, Delegate>>> entityEvents = new Dictionary<string, Dictionary<string, Dictionary<string, Delegate>>>();

    #region REGION_GLOBAL_EVENT_REGISTERATION

    private void DoRegisterGlobalEvent(string eventName, string callbackName, Delegate handler)
    {
        if (!globalEvents.ContainsKey(eventName))
        {
            globalEvents[eventName] = new Dictionary<string, Delegate>();
        }
        globalEvents[eventName].TryAdd(callbackName, handler);
    }

    public void RegisterGlobalEvent(string eventName, string callbackName, Action callback)
    {
        DoRegisterGlobalEvent(eventName, callbackName, callback);
    }
    public void RegisterGlobalEvent<T1>(string eventName, string callbackName, Action<T1> callback)
    {
        DoRegisterGlobalEvent(eventName, callbackName, callback);
    }
    public void RegisterGlobalEvent<T1, T2>(string eventName, string callbackName, Action<T1, T2> callback)
    {
        DoRegisterGlobalEvent(eventName, callbackName, callback);
    }
    public void RegisterGlobalEvent<T1, T2, T3>(string eventName, string callbackName, Action<T1, T2, T3> callback)
    {
        DoRegisterGlobalEvent(eventName, callbackName, callback);
    }

    public void UnregisterGlobalEvent(string eventName, string callbackName)
    {
        if (globalEvents.TryGetValue(eventName, out var eventHandlers))
        {
            eventHandlers.Remove(callbackName);
            if (eventHandlers.Count == 0)
            {
                globalEvents.Remove(eventName);
            }
        }
    }

    #endregion

    #region REGION_GLOBAL_EVENT_TRIGGER

    public void TriggerGlobalEvent(string eventName)
    {
        if (globalEvents.TryGetValue(eventName, out var events))
        {
            if (events != null)
            {
                foreach (Delegate d in events.Values)
                {
                    if (d is Action callback)
                    {
                        callback();
                    }
                }
            }
        }
    }
    public void TriggerGlobalEvent<T1>(string eventName, T1 t1)
    {
        if (globalEvents.TryGetValue(eventName, out var events))
        {
            if (events != null)
            {
                foreach (Delegate d in events.Values)
                {
                    if (d is Action<T1> callback)
                    {
                        callback(t1);
                    }
                }
            }
        }
    }
    public void TriggerGlobalEvent<T1, T2>(string eventName, T1 t1, T2 t2)
    {
        if (globalEvents.TryGetValue(eventName, out var events))
        {
            if (events != null)
            {
                foreach (Delegate d in events.Values)
                {
                    if (d is Action<T1, T2> callback)
                    {
                        callback(t1, t2);
                    }
                }
            }
        }
    }
    public void TriggerGlobalEvent<T1, T2, T3>(string eventName, T1 t1, T2 t2, T3 t3)
    {
        if (globalEvents.TryGetValue(eventName, out var events))
        {
            if (events != null)
            {
                foreach (Delegate d in events.Values)
                {
                    if (d is Action<T1, T2, T3> callback)
                    {
                        callback(t1, t2, t3);
                    }
                }
            }
        }
    }

    #endregion

    #region REGION_ENTITY_EVENT_REGISTRATION

    private void DoRegisterEntityEvent(string eid, string eventName, string callbackName, Delegate handler)
    {
        if (!entityEvents.ContainsKey(eid)) entityEvents[eid] = new Dictionary<string, Dictionary<string, Delegate>>();
        if (!entityEvents[eid].ContainsKey(eventName)) entityEvents[eid][eventName] = new Dictionary<string, Delegate>();
        entityEvents[eid][eventName].TryAdd(callbackName, handler);
    }

    public void RegisterEntityEvent(string eid, string eventName, string callbackName, Action callback)
    {
        DoRegisterEntityEvent(eid, eventName, callbackName, callback);
    }
    public void RegisterEntityEvent<T1>(string eid, string eventName, string callbackName, Action<T1> callback)
    {
        DoRegisterEntityEvent(eid, eventName, callbackName, callback);
    }
    public void RegisterEntityEvent<T1, T2>(string eid, string eventName, string callbackName, Action<T1, T2> callback)
    {
        DoRegisterEntityEvent(eid, eventName, callbackName, callback);
    }
    public void RegisterEntityEvent<T1, T2, T3>(string eid, string eventName, string callbackName, Action<T1, T2, T3> callback)
    {
        DoRegisterEntityEvent(eid, eventName, callbackName, callback);
    }

    public void UnregisterEntityEvent(string eid, string eventName, string callbackName)
    {
        if (entityEvents.TryGetValue(eid, out var events))
        {
            if (events.TryGetValue(eventName, out var eventHandlers))
            {
                eventHandlers.Remove(callbackName);
                if (eventHandlers.Count == 0)
                {
                    events.Remove(eventName);
                }
            }
            if (events.Count == 0)
            {
                entityEvents.Remove(eid);
            }
        }
    }

    #endregion

    #region REGION_ENTITY_EVENT_TRIGGER

    public void TriggerEntityEvent(string eid, string eventName)
    {
        if (entityEvents.TryGetValue(eid, out var events))
        {
            if (events.TryGetValue(eventName, out var eventHandlers))
            {
                foreach (Delegate d in eventHandlers.Values)
                {
                    if (d is Action callback)
                    {
                        callback();
                    }
                }
            }
        }
    }
    public void TriggerEntityEvent<T1>(string eid, string eventName, T1 t1)
    {
        if (entityEvents.TryGetValue(eid, out var events))
        {
            if (events.TryGetValue(eventName, out var eventHandlers))
            {
                foreach (Delegate d in eventHandlers.Values)
                {
                    if (d is Action<T1> callback)
                    {
                        callback(t1);
                    }
                }
            }
        }
    }
    public void TriggerEntityEvent<T1, T2>(string eid, string eventName, T1 t1, T2 t2)
    {
        if (entityEvents.TryGetValue(eid, out var events))
        {
            if (events.TryGetValue(eventName, out var eventHandlers))
            {
                foreach (Delegate d in eventHandlers.Values)
                {
                    if (d is Action<T1, T2> callback)
                    {
                        callback(t1, t2);
                    }
                }
            }
        }
    }
    public void TriggerEntityEvent<T1, T2, T3>(string eid, string eventName, T1 t1, T2 t2, T3 t3)
    {
        if (entityEvents.TryGetValue(eid, out var events))
        {
            if (events.TryGetValue(eventName, out var eventHandlers))
            {
                foreach (Delegate d in eventHandlers.Values)
                {
                    if (d is Action<T1, T2, T3> callback)
                    {
                        callback(t1, t2, t3);
                    }
                }
            }
        }
    }

    #endregion
}
