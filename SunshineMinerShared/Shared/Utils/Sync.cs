using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public static class SyncConst
{
    public const int OwnClient = (1 << 0);
    private const int OtherClient = (1 << 1);

    public const int AllClient = (OwnClient | OtherClient);
}

[AttributeUsage(AttributeTargets.Class)]
public class EntitySyncAttribute : Attribute
{
    public int syncType { get; }
    public EntitySyncAttribute(int syncType_)
    {
        syncType = syncType_;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class PropertySyncAttribute : Attribute
{
    public int syncType;
    public PropertySyncAttribute(int syncType_)
    {
        syncType = syncType_;
    }
}

public class SyncDataConst
{
    public const int DataTypeUndefined = 0;
    public const int DataTypeInt = 1;
    public const int DataTypeFloat = 2;
    public const int DataTypeString = 3;
    public const int DataTypeBool = 4;

    public static readonly int[] DataTypeLeaf = new int[]
    {
        DataTypeInt,
        DataTypeFloat,
        DataTypeString,
        DataTypeBool
    };

    public const int DataTypeList = 10;
    public const int DataTypeListTail = 11;
    public const int DataTypeDictionary = 12;
    public const int DataTypeDictionaryTail = 13;
    
    public const int DataTypeVector3 = 14;
    public const int DataTypePath = 15;
}

public class SyncDataNode
{
    /*
    * For SyncDataLeafNode, if immutable, value is unchangable after constructor.
    * For SyncDataBranchNode, if immutable, children nodes are unchangable after constructor.
    */
    protected bool immutable = false;

    /*
    * Used for type checks of parameters of RPC methods.
    */
    public int dataType
    {
        get
        {
            return GetDataType();
        }
    }
    protected virtual int GetDataType()
    {
        return SyncDataConst.DataTypeUndefined;
    }

    /*
    * Serialize wrapper data ingo writer with necessary appendix.
    */
    public virtual void Serialize(BinaryWriter writer)
    {
        return;
    }

    /*
    * Customized format in string
    */
    public override string ToString()
    {
        return "SyncDataNode(Null)";
    }

}

public class SyncDataLeafNode : SyncDataNode
{
    public Action<SyncDataNode, SyncDataNode>? OnSetter = null;
}

public class SyncDataBranchNode : SyncDataNode
{

}

public class SyncDataIntNode : SyncDataLeafNode
{
    private int value = 0;

    public SyncDataIntNode() {}
    public SyncDataIntNode(int value_)
    {
        value = value_;
    }
    public SyncDataIntNode(int value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return SyncDataConst.DataTypeInt;
    }

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int value_)
    {
        if (immutable) return;
        int oldValue = value;
        value = value_;
        if (OnSetter != null)
        {
            SyncDataIntNode oldNode = new SyncDataIntNode(oldValue, true);
            OnSetter(oldNode, this);
        }
    }
    
    public static SyncDataIntNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new SyncDataIntNode(reader.ReadInt32());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataIntNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"SyncDataIntNode({value})";
    }
}

public class SyncDataFloatNode : SyncDataLeafNode
{
    private float value = 0f;

    public SyncDataFloatNode() {}
    public SyncDataFloatNode(float value_)
    {
        value = value_;
    }
    public SyncDataFloatNode(float value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return SyncDataConst.DataTypeFloat;
    }

    public float GetValue()
    {
        return value;
    }

    public void SetValue(float value_)
    {
        if (immutable) return;
        float oldValue = value;
        value = value_;
        if (OnSetter != null)
        {
            SyncDataFloatNode oldNode = new SyncDataFloatNode(oldValue, true);
            OnSetter(oldNode, this);
        }
    }
    
    public static SyncDataFloatNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new SyncDataFloatNode(reader.ReadSingle());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataFloatNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"SyncDataFloatNode({value})";
    }
}

public class SyncDataStringNode : SyncDataLeafNode
{
    private string value = "";

    public SyncDataStringNode() {}
    public SyncDataStringNode(string value_)
    {
        value = value_;
    }
    public SyncDataStringNode(string value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return SyncDataConst.DataTypeString;
    }

    public string GetValue()
    {
        return value;
    }

    public void SetValue(string value_)
    {
        if (immutable) return;
        value = value_;
    }
    
    public static SyncDataStringNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new SyncDataStringNode(reader.ReadString());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataStringNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"SyncDataStringNode({value})";
    }
}

public class SyncDataBoolNode : SyncDataLeafNode
{
    private bool value = false;

    public SyncDataBoolNode() {}
    public SyncDataBoolNode(bool value_)
    {
        value = value_;
    }
    public SyncDataBoolNode(bool value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return SyncDataConst.DataTypeBool;
    }

    public bool GetValue()
    {
        return value;
    }

    public void SetValue(bool value_)
    {
        if (immutable) return;
        value = value_;
    }
    
    public static SyncDataBoolNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new SyncDataBoolNode(reader.ReadBoolean());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataBoolNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"SyncDataBoolNode({value})";
    }
}

public class SyncDataListTailNode: SyncDataNode
{
    protected override int GetDataType()
    {
        return SyncDataConst.DataTypeListTail;
    }
    
    public static SyncDataListTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new SyncDataListTailNode();
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataListTailNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        return;
    }

    public override string ToString()
    {
        return "SyncDataListTailNode(null)";
    }
}

public class SyncDataListNode : SyncDataBranchNode, IEnumerable<SyncDataNode>
{
    private List<SyncDataNode> children = new List<SyncDataNode>();

    public SyncDataListNode() {}
    public SyncDataListNode(bool immutable_)
    {
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return SyncDataConst.DataTypeList;
    }

    #region REGION_LIST_API

    public SyncDataNode this[int index]
    {
        get
        {
            if (index < 0 || index >= children.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            return children[index];
        }
        set
        {
            if (immutable) return;
            if (index < 0 || index >= children.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            children[index] = value;
        }
    }

    public int Count
    {
        get { return children.Count; }
    }

    public bool Contains(SyncDataNode child)
    {
        return children.Contains(child);
    }

    public int IndexOf(SyncDataNode child)
    {
        return children.IndexOf(child);
    }

    public SyncDataNode[] ToArray()
    {
        return children.ToArray();
    }

    public IEnumerator<SyncDataNode> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(SyncDataNode child)
    {
        if (immutable) return;
        children.Add(child);
    }

    public void Insert(int index, SyncDataNode child)
    {
        if (immutable) return;
        if (index < 0 || index > children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        children.Insert(index, child);
    }

    public void Remove(SyncDataNode child)
    {
        if (immutable) return;
        children.Remove(child);
    }

    public void RemoveAt(int index)
    {
        if (immutable) return;
        if (index < 0 || index >= children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        children.RemoveAt(index);
    }

    public void Clear()
    {
        if (immutable) return;
        children.Clear();
    }

    #endregion
    
    public static SyncDataListNode Deserialize(BinaryReader reader)
    {
        try
        {
            SyncDataListNode listNode = new SyncDataListNode();
            while (true)
            {
                SyncDataNode node = SyncStreamer.Deserialize(reader);
                if (node is SyncDataListTailNode tailNode) break;
                else listNode.Add(node);
            }
            return listNode;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataListNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        foreach (var child in children)
        {
            child.Serialize(writer);
        }
        writer.Write(SyncDataConst.DataTypeListTail);
    }

    public override string ToString()
    {
        string ls = "";
        foreach (var child in children)
        {
            ls += child.ToString() + ", ";
        }
        return $"SyncDataListNode([{ls}])";
    }
}

public class SyncDataDictionaryTailNode : SyncDataNode
{
    protected override int GetDataType()
    {
        return SyncDataConst.DataTypeDictionaryTail;
    }
    
    public static SyncDataDictionaryTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new SyncDataDictionaryTailNode();
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataDictionaryTailNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        return;
    }

    public override string ToString()
    {
        return "SyncDataDictionaryTailNode(null)";
    }
}

public class SyncDataDictionaryNode<TKey> : SyncDataBranchNode, IEnumerable<KeyValuePair<TKey, SyncDataNode>>
{
    private Dictionary<TKey, SyncDataNode> children = new Dictionary<TKey, SyncDataNode>();
    private int keyType = SyncDataConst.DataTypeUndefined;

    public SyncDataDictionaryNode()
    {
        SetKeyType();
    }
    public SyncDataDictionaryNode(bool immutable_)
    {
        immutable = immutable_;
        SetKeyType();
    }

    private void SetKeyType()
    {
        if (typeof(TKey) == typeof(int)) keyType = SyncDataConst.DataTypeInt;
        else if (typeof(TKey) == typeof(float)) keyType = SyncDataConst.DataTypeFloat;
        else if (typeof(TKey) == typeof(string)) keyType = SyncDataConst.DataTypeString;
    }

    protected override int GetDataType()
    {
        return SyncDataConst.DataTypeDictionary;
    }

    #region REGION_DICTIONARY_API

    public SyncDataNode this[TKey key]
    {
        get
        {
            if (!children.ContainsKey(key))
                throw new KeyNotFoundException($"Key '{key}' not found in dictionary.");
            return children[key];
        }
        set
        {
            if (immutable) return;
            children[key] = value;
        }
    }

    public int Count
    {
        get { return children.Count; }
    }

    public bool ContainsKey(TKey key)
    {
        return children.ContainsKey(key);
    }

    public bool TryGetValue(TKey key, out SyncDataNode value)
    {
        return children.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<TKey, SyncDataNode>> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator(); // Ĭ�Ϸ���KVP�汾��ö����
    }

    public void Add(TKey key, SyncDataNode value)
    {
        if (immutable) return;
        children.Add(key, value);
    }

    public bool Remove(TKey key)
    {
        if (immutable) return false;
        return children.Remove(key);
    }

    public void Clear()
    {
        if (immutable) return;
        children.Clear();
    }

    #endregion

    public static SyncDataDictionaryNode<TKey> Deserialize(BinaryReader reader)
    {
        try
        {
            SyncDataDictionaryNode<TKey> dictionary = new SyncDataDictionaryNode<TKey>();
            while (true)
            {
                SyncDataNode kNode = SyncStreamer.Deserialize(reader);
                if (kNode is SyncDataDictionaryTailNode tailNode) break;

                SyncDataNode vNode = SyncStreamer.Deserialize(reader);
                if (kNode is SyncDataIntNode kIntNode && kIntNode.GetValue() is TKey ik)
                {
                    dictionary.Add(ik, vNode);
                }
                else if (kNode is SyncDataFloatNode kFloatNode && kFloatNode.GetValue() is TKey fk)
                {
                    dictionary.Add(fk, vNode);
                }
                else if (kNode is SyncDataStringNode kStringNode && kStringNode.GetValue() is TKey sk)
                {
                    dictionary.Add(sk, vNode);
                }
                else throw new InvalidDataException($"Key type {kNode.GetType()} does not match expected type {typeof(TKey)}.");
            }
            return dictionary;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataDictionaryNode.", ex);
        }
    }
    
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(keyType);
        foreach (var kvp in children)
        {
            TKey key = kvp.Key;
            SyncDataNode value = kvp.Value;
            if (keyType == SyncDataConst.DataTypeInt)
            {
                if (key is int intKey)
                {
                    SyncDataIntNode intNode = new SyncDataIntNode(intKey);
                    intNode.Serialize(writer);
                }
                else throw new InvalidOperationException("Key type does not match expected type.");
            }
            else if (keyType == SyncDataConst.DataTypeFloat)
            {
                if (key is float floatKey)
                {
                    SyncDataFloatNode floatNode = new SyncDataFloatNode(floatKey);
                    floatNode.Serialize(writer);
                }
                else throw new InvalidOperationException("Key type does not match expected type.");
            }
            else if (keyType == SyncDataConst.DataTypeString)
            {
                if (key is string stringKey)
                {
                    SyncDataStringNode stringNode = new SyncDataStringNode(stringKey);
                    stringNode.Serialize(writer);
                }
                else throw new InvalidOperationException("Key type does not match expected type.");
            }
            else throw new InvalidOperationException($"Unsupported key type: {keyType}");
            value.Serialize(writer);
        }
        writer.Write(SyncDataConst.DataTypeDictionaryTail);
    }

    public override string ToString()
    {
        string ds = "";
        foreach (var kvp in children)
        {
            ds += $"{kvp.Key}: {kvp.Value.ToString()}, ";
        }
        return "SyncDataDictionaryNode({" + $"{ds}" + "})";
    }
}

public class SyncDataVector3Node : SyncDataBranchNode
{
    private float x = 0f;
    private float y = 0f;
    private float z = 0f;
    
    public SyncDataVector3Node() {}
    public SyncDataVector3Node(float x_, float y_, float z_)
    {
        x = x_;
        y = y_;
        z = z_;
    }
    
    protected override int GetDataType()
    {
        return SyncDataConst.DataTypeVector3;
    }
    
    public static SyncDataVector3Node Deserialize(BinaryReader reader)
    {
        try
        {
            return new SyncDataVector3Node(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataVector3Node.", ex);
        }
    }
    
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
    }
    
    public override string ToString()
    {
        return $"SyncDataVector3Node({x},{y},{z})";
    }
}

public class SyncDataPathNode : SyncDataBranchNode
{
    private SyncDataListNode pathPoints = new SyncDataListNode();
    private SyncDataListNode pathYaws = new SyncDataListNode();
    private SyncDataDictionaryNode<string> pathAlias = new SyncDataDictionaryNode<string>();
    
    public SyncDataPathNode() {}
    public SyncDataPathNode(SyncDataListNode points, SyncDataListNode yaws, SyncDataDictionaryNode<string> alias)
    {
        pathPoints = points;
        pathYaws = yaws;
        pathAlias = alias;
    }
    
    protected override int GetDataType()
    {
        return SyncDataConst.DataTypePath;
    }
    
    public void AddPath(float x, float y, float z, float w)
    {
        pathPoints.Add(new SyncDataVector3Node(x, y, z));
        pathYaws.Add(new SyncDataFloatNode(w));
    }
    
    public void AddPathAlias(string name, string alias)
    {
        pathAlias[name] = new SyncDataStringNode(alias);
    }
    
    public static SyncDataPathNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new SyncDataPathNode(
                SyncStreamer.Deserialize(reader) as SyncDataListNode,
                SyncStreamer.Deserialize(reader) as SyncDataListNode,
                SyncStreamer.Deserialize(reader) as SyncDataDictionaryNode<string>
            );
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize SyncDataPathNode.", ex);
        }
    }
    
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        pathPoints.Serialize(writer);
        pathYaws.Serialize(writer);
        pathAlias.Serialize(writer);
    }
    
    public override string ToString()
    {
        string s = "SyncDataPathNode(";
        s += $"pathPoints: {pathPoints}, ";
        s += $"pathYaws: {pathYaws}";
        s += $"pathAlias: {pathAlias}";
        s += ")";
        return s;
    }
    
}

public class SyncStreamer
{
    public static byte[] Serialize(SyncDataNode node)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        node.Serialize(writer);
        byte[] data = stream.ToArray();
        return data;
    }
    
    public static SyncDataNode Deserialize(BinaryReader reader)
    {
        try
        {
           int type = reader.ReadInt32();
           if (type == SyncDataConst.DataTypeInt)
           {
               return SyncDataIntNode.Deserialize(reader);
           }
           else if (type == SyncDataConst.DataTypeFloat)
           {
               return SyncDataFloatNode.Deserialize(reader);
           }
           else if (type == SyncDataConst.DataTypeString)
           {
               return SyncDataStringNode.Deserialize(reader);
           }
           else if (type == SyncDataConst.DataTypeBool)
           {
               return SyncDataBoolNode.Deserialize(reader);
           }
           else if (type == SyncDataConst.DataTypeList)
           {
               return SyncDataListNode.Deserialize(reader);
           }
           else if (type == SyncDataConst.DataTypeListTail)
           {
               return SyncDataListTailNode.Deserialize(reader);
           }
           else if (type == SyncDataConst.DataTypeDictionary)
           {
                int keyType = reader.ReadInt32();
                if (keyType == SyncDataConst.DataTypeInt) return SyncDataDictionaryNode<int>.Deserialize(reader);
                else if (keyType == SyncDataConst.DataTypeFloat) return SyncDataDictionaryNode<float>.Deserialize(reader);
                else if (keyType == SyncDataConst.DataTypeString) return SyncDataDictionaryNode<string>.Deserialize(reader);
                else throw new InvalidDataException("Unsupported key type for dictionary deserialization.");
           }
           else if (type == SyncDataConst.DataTypeDictionaryTail)
           {
               return SyncDataDictionaryTailNode.Deserialize(reader);
           }
           else if (type == SyncDataConst.DataTypeVector3)
           {
               return SyncDataVector3Node.Deserialize(reader);
           }
           else if (type == SyncDataConst.DataTypePath)
           {
               return SyncDataPathNode.Deserialize(reader);
           }
           else throw new InvalidDataException($"Unsupported data type: {type}");
        }
        catch
        {
            throw new InvalidDataException("Failed to deserialize SyncDataNode.");
        }
    }

    public static SyncDataDictionaryNode<string> SerializeProperties(object instance, int syncType)
    {
        SyncDataDictionaryNode<string> dict = new SyncDataDictionaryNode<string>();
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
                    dict.Add(property.Name, (SyncDataNode)value);
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
                    dict.Add(field.Name, (SyncDataNode)value);
                }
            }
        }
        return dict;
    }
}

/* TEST
public class TestCase
{
    public static void Main(string[] args)
    {
        SyncDataDictionaryNode<int> intD = new SyncDataDictionaryNode<int>();
        intD.Add(1, new SyncDataIntNode(1));
        intD.Add(2, new SyncDataFloatNode(2f));
        intD.Add(3, new SyncDataStringNode("3"));
        
        SyncDataDictionaryNode<float> floatD = new SyncDataDictionaryNode<float>();
        floatD[1.1f] = new SyncDataIntNode(11);
        floatD[2.2f] = new SyncDataFloatNode(22f);
        floatD[3.3f] = new SyncDataStringNode("33");
        
        SyncDataListNode l = new SyncDataListNode();
        l.Add(floatD);
        intD[4] = l;

        SyncDataDictionaryNode<string> stringD = new SyncDataDictionaryNode<string>();
        stringD["a"] = new SyncDataIntNode(111);
        stringD["b"] = new SyncDataFloatNode(222f);
        stringD["c"] = new SyncDataStringNode("333");
        intD[5] = stringD;
        
        intD[6] = new SyncDataVector3Node(1f, 2f, 3f);
        
        SyncDataPathNode path = new SyncDataPathNode();
        path.AddPath(0.1f, 0.2f, 0.3f, 1.57f);
        path.AddPath(1.1f, 1.2f, 1.3f, 3.14f);
        path.AddPath(2.1f, 2.2f, 2.3f, 0.0f);
        path.AddPathAlias("n1", "a1");
        path.AddPathAlias("n2", "a2");
        path.AddPathAlias("n3", "a3");
        intD[7] = path;
        
        Console.WriteLine ($"intD is {intD}");
        
        byte[] bd = SyncStreamer.Serialize(intD);
        using var stream = new MemoryStream(bd);
        using var reader = new BinaryReader(stream);
        SyncDataNode intDD = SyncStreamer.Deserialize(reader);
        Console.WriteLine ($"intDD is {intDD}");

        // Assert {intD} == {intDD}

        Console.WriteLine ("Over...");
    }   
}
 */
