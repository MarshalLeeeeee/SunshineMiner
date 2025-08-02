using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class Factory
{
    private static Dictionary<string, Type> componentTypes;

    public static void Init()
    {
        componentTypes = new Dictionary<string, Type>();
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type t in types)
        {
            string typeName = t.Name;
            if (t.IsSubclassOf(typeof(Component)))
            {
                componentTypes[typeName] = t;
            }
        }
    }

    public static Component? CreateComponent(string compName)
    {
        if (componentTypes.TryGetValue(compName, out Type compType))
        {
            return (Component)Activator.CreateInstance(compType);
        }
        return null;
    }
}
