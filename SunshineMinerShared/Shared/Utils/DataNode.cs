using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


public class DataNodeConst
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


public class DataNode
{
    /*
    * For DataLeafNode, if immutable, value is unchangable after constructor.
    * For DataBranchNode, if immutable, children nodes are unchangable after constructor.
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
        return DataNodeConst.DataTypeUndefined;
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
        return "DataNode(Null)";
    }

}

public class DataLeafNode : DataNode
{
    public Action<DataNode, DataNode>? OnSetter = null;
}

public class DataBranchNode : DataNode
{

}

public class DataIntNode : DataLeafNode
{
    private int value = 0;

    public DataIntNode() {}
    public DataIntNode(int value_)
    {
        value = value_;
    }
    public DataIntNode(int value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return DataNodeConst.DataTypeInt;
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
            DataIntNode oldNode = new DataIntNode(oldValue, true);
            OnSetter(oldNode, this);
        }
    }
    
    public static DataIntNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new DataIntNode(reader.ReadInt32());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataIntNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"DataIntNode({value})";
    }
}

public class DataFloatNode : DataLeafNode
{
    private float value = 0f;

    public DataFloatNode() {}
    public DataFloatNode(float value_)
    {
        value = value_;
    }
    public DataFloatNode(float value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return DataNodeConst.DataTypeFloat;
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
            DataFloatNode oldNode = new DataFloatNode(oldValue, true);
            OnSetter(oldNode, this);
        }
    }
    
    public static DataFloatNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new DataFloatNode(reader.ReadSingle());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataFloatNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"DataFloatNode({value})";
    }
}

public class DataStringNode : DataLeafNode
{
    private string value = "";

    public DataStringNode() {}
    public DataStringNode(string value_)
    {
        value = value_;
    }
    public DataStringNode(string value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return DataNodeConst.DataTypeString;
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
    
    public static DataStringNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new DataStringNode(reader.ReadString());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataStringNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"DataStringNode({value})";
    }
}

public class DataBoolNode : DataLeafNode
{
    private bool value = false;

    public DataBoolNode() {}
    public DataBoolNode(bool value_)
    {
        value = value_;
    }
    public DataBoolNode(bool value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return DataNodeConst.DataTypeBool;
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
    
    public static DataBoolNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new DataBoolNode(reader.ReadBoolean());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataBoolNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"DataBoolNode({value})";
    }
}

public class DataListTailNode: DataNode
{
    protected override int GetDataType()
    {
        return DataNodeConst.DataTypeListTail;
    }
    
    public static DataListTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new DataListTailNode();
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataListTailNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        return;
    }

    public override string ToString()
    {
        return "DataListTailNode(null)";
    }
}

public class DataListNode : DataBranchNode, IEnumerable<DataNode>
{
    private List<DataNode> children = new List<DataNode>();

    public DataListNode() {}
    public DataListNode(bool immutable_)
    {
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return DataNodeConst.DataTypeList;
    }

    #region REGION_LIST_API

    public DataNode this[int index]
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

    public bool Contains(DataNode child)
    {
        return children.Contains(child);
    }

    public int IndexOf(DataNode child)
    {
        return children.IndexOf(child);
    }

    public DataNode[] ToArray()
    {
        return children.ToArray();
    }

    public IEnumerator<DataNode> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(DataNode child)
    {
        if (immutable) return;
        children.Add(child);
    }

    public void Insert(int index, DataNode child)
    {
        if (immutable) return;
        if (index < 0 || index > children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        children.Insert(index, child);
    }

    public void Remove(DataNode child)
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
    
    public static DataListNode Deserialize(BinaryReader reader)
    {
        try
        {
            DataListNode listNode = new DataListNode();
            while (true)
            {
                DataNode node = SyncStreamer.Deserialize(reader);
                if (node is DataListTailNode tailNode) break;
                else listNode.Add(node);
            }
            return listNode;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataListNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        foreach (var child in children)
        {
            child.Serialize(writer);
        }
        writer.Write(DataNodeConst.DataTypeListTail);
    }

    public override string ToString()
    {
        string ls = "";
        foreach (var child in children)
        {
            ls += child.ToString() + ", ";
        }
        return $"DataListNode([{ls}])";
    }
}

public class DataDictionaryTailNode : DataNode
{
    protected override int GetDataType()
    {
        return DataNodeConst.DataTypeDictionaryTail;
    }
    
    public static DataDictionaryTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new DataDictionaryTailNode();
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataDictionaryTailNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        return;
    }

    public override string ToString()
    {
        return "DataDictionaryTailNode(null)";
    }
}

public class DataDictionaryNode<TKey> : DataBranchNode, IEnumerable<KeyValuePair<TKey, DataNode>>
{
    private Dictionary<TKey, DataNode> children = new Dictionary<TKey, DataNode>();
    private int keyType = DataNodeConst.DataTypeUndefined;

    public DataDictionaryNode()
    {
        SetKeyType();
    }
    public DataDictionaryNode(bool immutable_)
    {
        immutable = immutable_;
        SetKeyType();
    }

    private void SetKeyType()
    {
        if (typeof(TKey) == typeof(int)) keyType = DataNodeConst.DataTypeInt;
        else if (typeof(TKey) == typeof(float)) keyType = DataNodeConst.DataTypeFloat;
        else if (typeof(TKey) == typeof(string)) keyType = DataNodeConst.DataTypeString;
    }

    protected override int GetDataType()
    {
        return DataNodeConst.DataTypeDictionary;
    }

    #region REGION_DICTIONARY_API

    public DataNode this[TKey key]
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

    public bool TryGetValue(TKey key, out DataNode value)
    {
        return children.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<TKey, DataNode>> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator(); // Ĭ�Ϸ���KVP�汾��ö����
    }

    public void Add(TKey key, DataNode value)
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

    public static DataDictionaryNode<TKey> Deserialize(BinaryReader reader)
    {
        try
        {
            DataDictionaryNode<TKey> dictionary = new DataDictionaryNode<TKey>();
            while (true)
            {
                DataNode kNode = SyncStreamer.Deserialize(reader);
                if (kNode is DataDictionaryTailNode tailNode) break;

                DataNode vNode = SyncStreamer.Deserialize(reader);
                if (kNode is DataIntNode kIntNode && kIntNode.GetValue() is TKey ik)
                {
                    dictionary.Add(ik, vNode);
                }
                else if (kNode is DataFloatNode kFloatNode && kFloatNode.GetValue() is TKey fk)
                {
                    dictionary.Add(fk, vNode);
                }
                else if (kNode is DataStringNode kStringNode && kStringNode.GetValue() is TKey sk)
                {
                    dictionary.Add(sk, vNode);
                }
                else throw new InvalidDataException($"Key type {kNode.GetType()} does not match expected type {typeof(TKey)}.");
            }
            return dictionary;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataDictionaryNode.", ex);
        }
    }
    
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(keyType);
        foreach (var kvp in children)
        {
            TKey key = kvp.Key;
            DataNode value = kvp.Value;
            if (keyType == DataNodeConst.DataTypeInt)
            {
                if (key is int intKey)
                {
                    DataIntNode intNode = new DataIntNode(intKey);
                    intNode.Serialize(writer);
                }
                else throw new InvalidOperationException("Key type does not match expected type.");
            }
            else if (keyType == DataNodeConst.DataTypeFloat)
            {
                if (key is float floatKey)
                {
                    DataFloatNode floatNode = new DataFloatNode(floatKey);
                    floatNode.Serialize(writer);
                }
                else throw new InvalidOperationException("Key type does not match expected type.");
            }
            else if (keyType == DataNodeConst.DataTypeString)
            {
                if (key is string stringKey)
                {
                    DataStringNode stringNode = new DataStringNode(stringKey);
                    stringNode.Serialize(writer);
                }
                else throw new InvalidOperationException("Key type does not match expected type.");
            }
            else throw new InvalidOperationException($"Unsupported key type: {keyType}");
            value.Serialize(writer);
        }
        writer.Write(DataNodeConst.DataTypeDictionaryTail);
    }

    public override string ToString()
    {
        string ds = "";
        foreach (var kvp in children)
        {
            ds += $"{kvp.Key}: {kvp.Value.ToString()}, ";
        }
        return "DataDictionaryNode({" + $"{ds}" + "})";
    }
}

public class DataVector3Node : DataBranchNode
{
    private float x = 0f;
    private float y = 0f;
    private float z = 0f;
    
    public DataVector3Node() {}
    public DataVector3Node(float x_, float y_, float z_)
    {
        x = x_;
        y = y_;
        z = z_;
    }
    
    protected override int GetDataType()
    {
        return DataNodeConst.DataTypeVector3;
    }
    
    public static DataVector3Node Deserialize(BinaryReader reader)
    {
        try
        {
            return new DataVector3Node(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataVector3Node.", ex);
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
        return $"DataVector3Node({x},{y},{z})";
    }
}

public class DataPathNode : DataBranchNode
{
    private DataListNode pathPoints = new DataListNode();
    private DataListNode pathYaws = new DataListNode();
    private DataDictionaryNode<string> pathAlias = new DataDictionaryNode<string>();
    
    public DataPathNode() {}
    public DataPathNode(DataListNode points, DataListNode yaws, DataDictionaryNode<string> alias)
    {
        pathPoints = points;
        pathYaws = yaws;
        pathAlias = alias;
    }
    
    protected override int GetDataType()
    {
        return DataNodeConst.DataTypePath;
    }
    
    public void AddPath(float x, float y, float z, float w)
    {
        pathPoints.Add(new DataVector3Node(x, y, z));
        pathYaws.Add(new DataFloatNode(w));
    }
    
    public void AddPathAlias(string name, string alias)
    {
        pathAlias[name] = new DataStringNode(alias);
    }
    
    public static DataPathNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new DataPathNode(
                SyncStreamer.Deserialize(reader) as DataListNode,
                SyncStreamer.Deserialize(reader) as DataListNode,
                SyncStreamer.Deserialize(reader) as DataDictionaryNode<string>
            );
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize DataPathNode.", ex);
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
        string s = "DataPathNode(";
        s += $"pathPoints: {pathPoints}, ";
        s += $"pathYaws: {pathYaws}";
        s += $"pathAlias: {pathAlias}";
        s += ")";
        return s;
    }
    
}

public class SyncStreamer
{
    public static byte[] Serialize(DataNode node)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        node.Serialize(writer);
        byte[] data = stream.ToArray();
        return data;
    }
    
    public static DataNode Deserialize(BinaryReader reader)
    {
        try
        {
           int type = reader.ReadInt32();
           if (type == DataNodeConst.DataTypeInt)
           {
               return DataIntNode.Deserialize(reader);
           }
           else if (type == DataNodeConst.DataTypeFloat)
           {
               return DataFloatNode.Deserialize(reader);
           }
           else if (type == DataNodeConst.DataTypeString)
           {
               return DataStringNode.Deserialize(reader);
           }
           else if (type == DataNodeConst.DataTypeBool)
           {
               return DataBoolNode.Deserialize(reader);
           }
           else if (type == DataNodeConst.DataTypeList)
           {
               return DataListNode.Deserialize(reader);
           }
           else if (type == DataNodeConst.DataTypeListTail)
           {
               return DataListTailNode.Deserialize(reader);
           }
           else if (type == DataNodeConst.DataTypeDictionary)
           {
                int keyType = reader.ReadInt32();
                if (keyType == DataNodeConst.DataTypeInt) return DataDictionaryNode<int>.Deserialize(reader);
                else if (keyType == DataNodeConst.DataTypeFloat) return DataDictionaryNode<float>.Deserialize(reader);
                else if (keyType == DataNodeConst.DataTypeString) return DataDictionaryNode<string>.Deserialize(reader);
                else throw new InvalidDataException("Unsupported key type for dictionary deserialization.");
           }
           else if (type == DataNodeConst.DataTypeDictionaryTail)
           {
               return DataDictionaryTailNode.Deserialize(reader);
           }
           else if (type == DataNodeConst.DataTypeVector3)
           {
               return DataVector3Node.Deserialize(reader);
           }
           else if (type == DataNodeConst.DataTypePath)
           {
               return DataPathNode.Deserialize(reader);
           }
           else throw new InvalidDataException($"Unsupported data type: {type}");
        }
        catch
        {
            throw new InvalidDataException("Failed to deserialize DataNode.");
        }
    }

    public static DataDictionaryNode<string> SerializeProperties(object instance, int syncType)
    {
        DataDictionaryNode<string> dict = new DataDictionaryNode<string>();
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
                    dict.Add(property.Name, (DataNode)value);
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
                    dict.Add(field.Name, (DataNode)value);
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
        DataDictionaryNode<int> intD = new DataDictionaryNode<int>();
        intD.Add(1, new DataIntNode(1));
        intD.Add(2, new DataFloatNode(2f));
        intD.Add(3, new DataStringNode("3"));
        
        DataDictionaryNode<float> floatD = new DataDictionaryNode<float>();
        floatD[1.1f] = new DataIntNode(11);
        floatD[2.2f] = new DataFloatNode(22f);
        floatD[3.3f] = new DataStringNode("33");
        
        DataListNode l = new DataListNode();
        l.Add(floatD);
        intD[4] = l;

        DataDictionaryNode<string> stringD = new DataDictionaryNode<string>();
        stringD["a"] = new DataIntNode(111);
        stringD["b"] = new DataFloatNode(222f);
        stringD["c"] = new DataStringNode("333");
        intD[5] = stringD;
        
        intD[6] = new DataVector3Node(1f, 2f, 3f);
        
        DataPathNode path = new DataPathNode();
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
        DataNode intDD = SyncStreamer.Deserialize(reader);
        Console.WriteLine ($"intDD is {intDD}");

        // Assert {intD} == {intDD}

        Console.WriteLine ("Over...");
    }   
}
*/
