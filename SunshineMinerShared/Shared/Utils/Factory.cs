using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public static class Factory
{
    /* component types */
    private static Dictionary<string, Type> componentTypes = new Dictionary<string, Type>();
    /* prop node types */
    private static Dictionary<int, Type> propNodeTypes = new Dictionary<int, Type>();

    public static void Init()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type t in types)
        {
            string typeName = t.Name;

            // register Component
            CompAttribute? compAttr = t.GetCustomAttribute<CompAttribute>();
            if (compAttr != null)
            {
                componentTypes[typeName] = t;
            }

            // register PropNode
            if (t.IsSubclassOf(typeof(PropNode)))
            {
                FieldInfo[] staticFields = t.GetFields(
                    BindingFlags.Static |
                    BindingFlags.Public
                );
                foreach (FieldInfo staticField in staticFields)
                {
                    if (staticField.Name == "staticPropType")
                    {
                        object? obj = staticField.GetValue(null);
                        if (obj != null && obj is int propType && propType != PropNodeConst.TypeUndefined)
                        {
                            propNodeTypes[propType] = t;
                        }
                    }
                }
            }
        }
    }

    /* Create default component by name */
    public static Component? CreateComponent(string compName)
    {
        if (componentTypes.TryGetValue(compName, out Type compType))
        {
            return (Component)Activator.CreateInstance(compType);
        }
        return null;
    }

    /* Get Deserialize method given prop node type */
    public static MethodInfo? GetDeserializeMethod(int propType)
    {
        if (propNodeTypes.TryGetValue(propType, out Type propNodeType))
        {
            MethodInfo? method = propNodeType.GetMethod(
                "Deserialize",
                BindingFlags.Static | BindingFlags.Public
            );
            return method;
        }
        return null;
    }
}
