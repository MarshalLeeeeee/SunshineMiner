using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

static public class CustomTypeConst
{
    public const int TypeUndefined = 0;
    public const int TypeInt = 1;
    public const int TypeFloat = 2;
    public const int TypeString = 3;

    public const int TypeList = 124;
    public const int TypeListTail = 125;
    public const int TypeDict = 126;
    public const int TypeDictTail = 127;
}

public class CustomType
{
    public int type = CustomTypeConst.TypeUndefined;
    protected object obj;

    //public static void SerializeFactory(CustomType arg, BinaryWriter writer)
    //{
    //    switch (arg.type)
    //    {
    //        case CustomTypeConst.TypeInt:
    //            (arg as CustomInt).Serialize(writer);
    //            return;
    //        case CustomTypeConst.TypeFloat:
    //            (arg as ArgFloat).Serialize(writer);
    //            return;
    //        case CustomTypeConst.TypeString:
    //            (arg as ArgString).Serialize(writer);
    //            return;
    //        case CustomTypeConst.TypeList:
    //            (arg as ArgList).Serialize(writer);
    //            return;
    //        case CustomTypeConst.TypeDict:
    //            (arg as ArgDict).Serialize(writer);
    //            return;
    //        default:
    //            (arg as CustomType).Serialize(writer);
    //            return;
    //    }
    //}

    public virtual void Serialize(BinaryWriter writer)
    {
        return;
    }

    //public static void PrintFactory(CustomType arg)
    //{
    //    switch (arg.type)
    //    {
    //        case CustomTypeConst.TypeInt:
    //            (arg as CustomInt).Print();
    //            return;
    //        case CustomTypeConst.TypeFloat:
    //            (arg as ArgFloat).Print();
    //            return;
    //        case CustomTypeConst.TypeString:
    //            (arg as ArgString).Print();
    //            return;
    //        case CustomTypeConst.TypeList:
    //            (arg as ArgList).Print();
    //            return;
    //        case CustomTypeConst.TypeDict:
    //            (arg as ArgDict).Print();
    //            return;
    //        default:
    //            (arg as CustomType).Print();
    //            return;
    //    }
    //}

    public virtual void Print()
    {
        return;
    }
}
public class CustomInt : CustomType
{
    public CustomInt(int v = 0)
    {
        type = CustomTypeConst.TypeInt;
        obj = v;
    }

    public void Setter(int v)
    {
        obj = v;
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

    public override void Print()
    {
        Console.Write($"{(int)obj}");
    }
}
public class CustomFloat : CustomType
{
    public CustomFloat(float v = 0f)
    {
        type = CustomTypeConst.TypeFloat;
        obj = v;
    }

    public void Setter(float v)
    {
        obj = v;
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

    public override void Print()
    {
        Console.Write($"{(float)obj}");
    }
}
public class CustomString : CustomType
{
    public CustomString(string v = "")
    {
        type = CustomTypeConst.TypeString;
        obj = v;
    }

    public void Setter(string v)
    {
        obj = v;
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

    public override void Print()
    {
        Console.Write($"\"{(string)obj}\"");
    }
}
public class CustomList : CustomType
{
    public CustomList()
    {
        type = CustomTypeConst.TypeList;
        obj = new List<CustomType>();
    }

    public void Add<T>(T arg) where T : CustomType
    {
        (obj as List<CustomType>).Add(arg);
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(CustomTypeConst.TypeList);
        foreach (CustomType arg in (obj as List<CustomType>))
        {
            arg.Serialize(writer);
        }
        writer.Write(CustomTypeConst.TypeListTail);
    }

    public override void Print()
    {
        Console.Write("[");
        foreach (CustomType arg in (obj as List<CustomType>))
        {
            arg.Print();
            Console.Write(", ");
        }
        Console.Write("]");
    }
}
public class CustomDict : CustomType
{
    public CustomDict()
    {
        type = CustomTypeConst.TypeDict;
        obj = new Dictionary<CustomType, CustomType>();
    }

    public void Add<T1, T2>(T1 key, T2 value) where T1 : CustomType where T2 : CustomType
    {
        (obj as Dictionary<CustomType, CustomType>).Add(key, value);
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(CustomTypeConst.TypeDict);
        foreach (var kvp in (obj as Dictionary<CustomType, CustomType>))
        {
            CustomType k = kvp.Key;
            CustomType v = kvp.Value;
            k.Serialize(writer);
            v.Serialize(writer);
        }
        writer.Write(CustomTypeConst.TypeDictTail);
    }

    public override void Print()
    {
        Console.Write("{");
        foreach (var kvp in (obj as Dictionary<CustomType, CustomType>))
        {
            CustomType k = kvp.Key;
            CustomType v = kvp.Value;
            k.Print();
            Console.Write(": ");
            v.Print();
            Console.Write(", ");
        }
        Console.Write("}");
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
}
