using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class EventManager : Manager
{
    private Dictionary<string, Dictionary<string, Delegate>> globalEvents = new Dictionary<string, Dictionary<string, Delegate>>();

    #region REGION_GLOBAL_EVENT_REGISTERATION

    private void DoRegisterGlobalEvent(string eventName, string callbackName, Delegate handler)
    {
        if (!globalEvents.ContainsKey(eventName))
        {
            globalEvents[eventName] = new Dictionary<string, Delegate>();
        }
        globalEvents[eventName].TryAdd(callbackName, handler);
        Console.WriteLine($"register event {eventName} {callbackName}");
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
        if (globalEvents.TryGetValue(eventName, out var events))
        {
            if (events != null)
            {
                events.Remove(callbackName);
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
}
