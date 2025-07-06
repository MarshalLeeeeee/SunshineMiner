using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

static public class CustomTypeConst
{
    public const int TypeUndefined = 0;
    public const int TypeInt = 1;
    public const int TypeFloat = 2;
    public const int TypeString = 3;
    public const int TypeBool = 4;

    public const int TypeList = 124;
    public const int TypeListTail = 125;
    public const int TypeDict = 126;
    public const int TypeDictTail = 127;

    public static readonly int[] TypePrimary = { TypeInt, TypeFloat, TypeString, TypeBool };
}

public class CustomType
{
    public int type = CustomTypeConst.TypeUndefined;
    protected object obj;
    public virtual void Serialize(BinaryWriter writer)
    {
        return;
    }

    public virtual string CustomToString()
    {
        return "";
    }
}
public class CustomInt : CustomType
{
    public Action<CustomType, CustomType>? OnSetter = null;
    public CustomInt(int v = 0)
    {
        type = CustomTypeConst.TypeInt;
        obj = v;
    }

    public void Setter(int v)
    {
        int old = (int)obj;
        if (old == v) return;
        obj = v;
        if (OnSetter != null)
        {
            OnSetter(new CustomInt(old), new CustomInt(v));
        }
    }

    public int Getter()
    {
        return (int)obj;
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(type);
        writer.Write((int)obj);
    }

    public override string CustomToString()
    {
        return $"{(int)obj}";
    }
}
public class CustomFloat : CustomType
{
    public Action<CustomType, CustomType>? OnSetter = null;
    public CustomFloat(float v = 0f)
    {
        type = CustomTypeConst.TypeFloat;
        obj = v;
    }

    public void Setter(float v)
    {
        float old = (float)obj;
        if (old == v) return;
        obj = v;
        if (OnSetter != null)
        {
            OnSetter(new CustomFloat(old), new CustomFloat(v));
        }
    }

    public float Getter()
    {
        return (float)obj;
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(type);
        writer.Write((float)obj);
    }

    public override string CustomToString()
    {
        return $"{(float)obj}";
    }
}
public class CustomString : CustomType
{
    public Action<CustomType>? OnSetter = null;
    public CustomString(string v = "")
    {
        type = CustomTypeConst.TypeString;
        obj = v;
    }

    public void Setter(string v)
    {
        if ((string)obj == v) return;
        obj = v;
        if (OnSetter != null)
        {
            OnSetter(this);
        }
    }

    public string Getter()
    {
        return (string)obj;
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(type);
        writer.Write((string)obj);
    }

    public override string CustomToString()
    {
        return $"\"{(string)obj}\"";
    }
}

public class CustomBool : CustomType
{
    public Action<CustomType>? OnSetter = null;
    public CustomBool(bool v = false)
    {
        type = CustomTypeConst.TypeBool;
        obj = v;
    }

    public void Setter(bool v)
    {
        if ((bool)obj == v) return;
        obj = v;
        if (OnSetter != null)
        {
            OnSetter(this);
        }
    }

    public bool Getter()
    {
        return (bool)obj;
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(type);
        writer.Write((bool)obj);
    }

    public override string CustomToString()
    {
        if ((bool)obj)
        {
           return "true";
        }
        else
        {
            return "false";
        }
    }
}

public class CustomList : CustomType, IEnumerable
{
    public Action<CustomType>? OnAdd = null;
    public Action<CustomType, CustomInt>? OnInsert = null;
    public Action<CustomType, CustomInt, CustomType>? OnRemove = null;
    public Action<CustomType>? OnClear = null;
    public Action<CustomType, CustomInt>? OnSet = null;
    public CustomList()
    {
        type = CustomTypeConst.TypeList;
        obj = new List<CustomType>();
    }

    public void Add(CustomType arg)
    {
        List<CustomType> l = (List<CustomType>)obj;
        l.Add(arg);
        if (OnAdd != null)
        {
            OnAdd(this);
        }
    }

    public void Insert(int index, CustomType arg)
    {
        List<CustomType> l = (List<CustomType>)obj;
        int count = l.Count;
        if (index < 0 || index > count) return; // Invalid index

        l.Insert(index, arg);
        if (OnInsert != null)
        {
            OnInsert(this, new CustomInt(index));
        }
    }

    public void Remove(CustomType arg)
    {
        List<CustomType> l = (List<CustomType>)obj;
        if (l.Contains(arg))
        {
            int index = l.IndexOf(arg);
            l.Remove(arg);
            if (OnRemove != null)
            {
                OnRemove(this, new CustomInt(index), arg);
            }
        }
    }

    public void RemoveAt(int index)
    {
        List<CustomType> l = (List<CustomType>)obj;
        if (index < 0 || index >= l.Count) return; // Invalid index

        CustomType arg = l[index];
        l.RemoveAt(index);
        if (OnRemove != null)
        {
            OnRemove(this, new CustomInt(index), arg);
        }
    }

    public void Clear()
    {
        List<CustomType> l = (List<CustomType>)obj;
        l.Clear();
        if (OnClear != null)
        {
            OnClear(this);
        }
    }

    public bool Contains(CustomType arg)
    {
        List<CustomType> l = (List<CustomType>)obj;
        return l.Contains(arg);
    }

    public int IndexOf(CustomType arg)
    {
        List<CustomType> l = (List<CustomType>)obj;
        return l.IndexOf(arg);
    }

    public CustomType this[int index]
    {
        get
        {
            List<CustomType> l = (List<CustomType>)obj;
            return l[index];
        }
        set
        {
            List<CustomType> l = (List<CustomType>)obj;
            l[index] = value;
            if (OnSet != null)
            {
                OnSet(this, new CustomInt(index));
            }
        }
    }

    public IEnumerator GetEnumerator()
    {
        List<CustomType> l = (List<CustomType>)obj;
        foreach (CustomType arg in l)
        {
            yield return arg;
        }
    }

    public int Count => ((List<CustomType>)obj).Count;

    public CustomType[] ToArray()
    {
        List<CustomType> l = (List<CustomType>)obj;
        return l.ToArray();
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(CustomTypeConst.TypeList);
        foreach (CustomType arg in ((List<CustomType>)obj))
        {
            arg.Serialize(writer);
        }
        writer.Write(CustomTypeConst.TypeListTail);
    }

    public override string CustomToString()
    {
        string ss = "";
        foreach (CustomType arg in ((List<CustomType>)obj))
        {
            ss += arg.CustomToString() + ", ";
        }
        return $"[{ss}]";
    }
}
public class CustomDict : CustomType, IEnumerable
{
    public Action<CustomType, CustomType>? OnSet = null;
    public Action<CustomType, CustomType>? OnRemove = null;
    public Action<CustomType>? OnClear = null;
    public CustomDict()
    {
        type = CustomTypeConst.TypeDict;
        obj = new Dictionary<CustomType, CustomType>();
    }

    public void Add(CustomType key, CustomType value)
    {
        ((Dictionary<CustomType, CustomType>)obj).Add(key, value);
        if (OnSet != null)
        {
            OnSet(this, key);
        }
    }

    public void Remove(CustomType key)
    {
        Dictionary<CustomType, CustomType> d = (Dictionary<CustomType, CustomType>)obj;
        if (d.ContainsKey(key))
        {
            d.Remove(key);
            if (OnRemove != null)
            {
                OnRemove(this, key);
            }
        }
    }

    public void Clear()
    {
        Dictionary<CustomType, CustomType> d = (Dictionary<CustomType, CustomType>)obj;
        d.Clear();
        if (OnClear != null)
        {
            OnClear(this);
        }
    }

    public CustomType this[CustomType key]
    {
        get
        {
            Dictionary <CustomType, CustomType> d = (Dictionary<CustomType, CustomType>)obj;
            foreach (var kvp in d)
            {
                if (Equals(kvp.Key, key))
                {
                    return kvp.Value;
                }
            }
            throw new KeyNotFoundException();
        }
        set
        {
            Dictionary<CustomType, CustomType> d = (Dictionary<CustomType, CustomType>)obj;
            foreach (var kvp in d)
            {
                if (Equals(kvp.Key, key))
                {
                    d[key] = value;
                    if (OnSet != null)
                    {
                        OnSet(this, key);
                    }
                    return;
                }
            }
            Add(key, value);
        }
    }

    public bool TryGetValue(CustomType key, out CustomType? value)
    {
        Dictionary<CustomType, CustomType> d = (Dictionary<CustomType, CustomType>)obj;
        if (d.TryGetValue(key, out value))
        {
            return true;
        }
        value = null;
        return false;
    }

    public bool ContainsKey(CustomType key)
    {
        Dictionary<CustomType, CustomType> d = (Dictionary<CustomType, CustomType>)obj;
        return d.ContainsKey(key);
    }

    public int Count => ((Dictionary<CustomType, CustomType>)obj).Count;

    public CustomType[] Keys
    {
        get
        {
            Dictionary<CustomType, CustomType> d = (Dictionary<CustomType, CustomType>)obj;
            return new List<CustomType>(d.Keys).ToArray();
        }
    }

    public CustomType[] Values
    {
        get
        {
            Dictionary<CustomType, CustomType> d = (Dictionary<CustomType, CustomType>)obj;
            return new List<CustomType>(d.Values).ToArray();
        }
    }

    public IEnumerator GetEnumerator()
    {
        Dictionary<CustomType, CustomType> d = (Dictionary<CustomType, CustomType>)obj;
        foreach (var kvp in d)
        {
            yield return new DictionaryEntry { Key = kvp.Key, Value = kvp.Value };
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(CustomTypeConst.TypeDict);
        foreach (var kvp in ((Dictionary<CustomType, CustomType>)obj))
        {
            CustomType k = kvp.Key;
            CustomType v = kvp.Value;
            k.Serialize(writer);
            v.Serialize(writer);
        }
        writer.Write(CustomTypeConst.TypeDictTail);
    }

    public override string CustomToString()
    {
        string ss = "";
        foreach (var kvp in ((Dictionary<CustomType, CustomType>)obj))
        {
            CustomType k = kvp.Key;
            CustomType v = kvp.Value;
            ss += k.CustomToString() + ": " + v.CustomToString() + ", ";
        }
        return "{" + $"{ss}" + "}";
    }
}

public class CustomTypeStreamer
{
    public static byte[] Serialize(CustomType arg)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        arg.Serialize(writer);
        byte[] data = stream.ToArray();
        return data;
    }

    private static void DeserializeHandleStacks(Stack<int> typeStack, Stack<CustomType> argStack, Stack<CustomType> argDk, CustomType arg)
    {
        int topType = typeStack.Peek();
        if (topType == CustomTypeConst.TypeList)
        {
            (argStack.Peek() as CustomList).Add(arg);
        }
        else if (topType == CustomTypeConst.TypeDict)
        {
            if (argDk.Peek() == null)
            {
                argDk.Pop();
                argDk.Push(arg);
            }
            else
            {
                (argStack.Peek() as CustomDict).Add(argDk.Pop(), arg);
                argDk.Push(null);
            }
        }
    }

    public static CustomType Deserialize(BinaryReader reader)
    {
        Stack<int> typeStack = new Stack<int>();
        Stack<CustomType> argStack = new Stack<CustomType>();
        Stack<CustomType> argDk = new Stack<CustomType>();
        CustomType arg;

        try
        {
            while (true)
            {
                int type = reader.ReadInt32();
                if (type == CustomTypeConst.TypeInt)
                {
                    arg = new CustomInt(reader.ReadInt32());
                    if (typeStack.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        DeserializeHandleStacks(typeStack, argStack, argDk, arg);
                    }
                }
                else if (type == CustomTypeConst.TypeFloat)
                {
                    arg = new CustomFloat(reader.ReadSingle());
                    if (typeStack.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        DeserializeHandleStacks(typeStack, argStack, argDk, arg);
                    }
                }
                else if (type == CustomTypeConst.TypeString)
                {
                    arg = new CustomString(reader.ReadString());
                    if (typeStack.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        DeserializeHandleStacks(typeStack, argStack, argDk, arg);
                    }
                }
                else if (type == CustomTypeConst.TypeBool)
                {
                    arg = new CustomBool(reader.ReadBoolean());
                    if (typeStack.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        DeserializeHandleStacks(typeStack, argStack, argDk, arg);
                    }
                }
                else if (type == CustomTypeConst.TypeList)
                {
                    typeStack.Push(CustomTypeConst.TypeList);
                    argStack.Push(new CustomList());
                }
                else if (type == CustomTypeConst.TypeListTail)
                {
                    typeStack.Pop();
                    arg = argStack.Pop();
                    if (typeStack.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        DeserializeHandleStacks(typeStack, argStack, argDk, arg);
                    }
                }
                else if (type == CustomTypeConst.TypeDict)
                {
                    typeStack.Push(CustomTypeConst.TypeDict);
                    argStack.Push(new CustomDict());
                    argDk.Push(null);
                }
                else if (type == CustomTypeConst.TypeDictTail)
                {
                    typeStack.Pop();
                    arg = argStack.Pop();
                    argDk.Pop();
                    if (typeStack.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        DeserializeHandleStacks(typeStack, argStack, argDk, arg);
                    }
                }
                else
                {
                    return new CustomType();
                }
            }
            return arg;
        }
        catch
        {
            return new CustomType();
        }
    }

    public static CustomDict SerializeProperties(object instance, int syncType)
    {
        CustomDict dict = new CustomDict();
        Type type = instance.GetType();
        var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var property in properties)
        {
            var attr = property.GetCustomAttribute<PropertySyncAttribute>();

            if (attr != null && (attr.syncType & syncType) != 0)
            {
                var value = property.GetValue(instance);
                if (value != null)
                {
                    dict.Add(new CustomString(property.Name), (CustomType)value);
                }
            }
        }
        var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<PropertySyncAttribute>();

            if (attr != null && (attr.syncType & syncType) != 0)
            {
                var value = field.GetValue(instance);
                if (value != null)
                {
                    dict.Add(new CustomString(field.Name), (CustomType)value);
                }
            }
        }
        return dict;
    }
}
