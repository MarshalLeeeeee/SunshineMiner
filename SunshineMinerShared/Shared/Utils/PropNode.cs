using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class PropNodeConst
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

public class PropNode
{
    /*
    * For PropLeafNode, if immutable, value is unchangable after constructor.
    * For PropBranchNode, if immutable, children nodes are unchangable after constructor.
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
        return PropNodeConst.DataTypeUndefined;
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
        return "PropNode(Null)";
    }

}

public class PropLeafNode : PropNode
{
    public Action<PropNode, PropNode>? OnSetter = null;
}

public class PropBranchNode : PropNode
{

}

public class PropIntNode : PropLeafNode
{
    private int value = 0;

    public PropIntNode() {}
    public PropIntNode(int value_)
    {
        value = value_;
    }
    public PropIntNode(int value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return PropNodeConst.DataTypeInt;
    }

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int value_)
    {
        if (immutable) return;
        int oldValue = value;
        if (oldValue == value_) return; // No change, no need to notify
        value = value_;
        if (OnSetter != null)
        {
            PropIntNode oldNode = new PropIntNode(oldValue, true);
            OnSetter(oldNode, this);
        }
    }
    
    public static PropIntNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new PropIntNode(reader.ReadInt32());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropIntNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"PropIntNode({value})";
    }
}

public class PropFloatNode : PropLeafNode
{
    private float value = 0f;

    public PropFloatNode() {}
    public PropFloatNode(float value_)
    {
        value = value_;
    }
    public PropFloatNode(float value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return PropNodeConst.DataTypeFloat;
    }

    public float GetValue()
    {
        return value;
    }

    public void SetValue(float value_)
    {
        if (immutable) return;
        float oldValue = value;
        if (Math.Abs(oldValue - value_) < 0.0001f) return; // No change, no need to notify
        value = value_;
        if (OnSetter != null)
        {
            PropFloatNode oldNode = new PropFloatNode(oldValue, true);
            OnSetter(oldNode, this);
        }
    }
    
    public static PropFloatNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new PropFloatNode(reader.ReadSingle());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropFloatNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"PropFloatNode({value})";
    }
}

public class PropStringNode : PropLeafNode
{
    private string value = "";

    public PropStringNode() {}
    public PropStringNode(string value_)
    {
        value = value_;
    }
    public PropStringNode(string value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return PropNodeConst.DataTypeString;
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
    
    public static PropStringNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new PropStringNode(reader.ReadString());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropStringNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"PropStringNode({value})";
    }
}

public class PropBoolNode : PropLeafNode
{
    private bool value = false;

    public PropBoolNode() {}
    public PropBoolNode(bool value_)
    {
        value = value_;
    }
    public PropBoolNode(bool value_, bool immutable_)
    {
        value = value_;
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return PropNodeConst.DataTypeBool;
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
    
    public static PropBoolNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new PropBoolNode(reader.ReadBoolean());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropBoolNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"PropBoolNode({value})";
    }
}

public class PropListTailNode: PropNode
{
    protected override int GetDataType()
    {
        return PropNodeConst.DataTypeListTail;
    }
    
    public static PropListTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new PropListTailNode();
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropListTailNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        return;
    }

    public override string ToString()
    {
        return "PropListTailNode(null)";
    }
}

public class PropListNode : PropBranchNode, IEnumerable<PropNode>
{
    private List<PropNode> children = new List<PropNode>();

    public PropListNode() {}
    public PropListNode(bool immutable_)
    {
        immutable = immutable_;
    }

    protected override int GetDataType()
    {
        return PropNodeConst.DataTypeList;
    }

    #region REGION_LIST_API

    public PropNode this[int index]
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

    public bool Contains(PropNode child)
    {
        return children.Contains(child);
    }

    public int IndexOf(PropNode child)
    {
        return children.IndexOf(child);
    }

    public PropNode[] ToArray()
    {
        return children.ToArray();
    }

    public IEnumerator<PropNode> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(PropNode child)
    {
        if (immutable) return;
        children.Add(child);
    }

    public void Insert(int index, PropNode child)
    {
        if (immutable) return;
        if (index < 0 || index > children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        children.Insert(index, child);
    }

    public void Remove(PropNode child)
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
    
    public static PropListNode Deserialize(BinaryReader reader)
    {
        try
        {
            PropListNode listNode = new PropListNode();
            while (true)
            {
                PropNode node = PropStreamer.Deserialize(reader);
                if (node is PropListTailNode tailNode) break;
                else listNode.Add(node);
            }
            return listNode;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropListNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        foreach (var child in children)
        {
            child.Serialize(writer);
        }
        writer.Write(PropNodeConst.DataTypeListTail);
    }

    public override string ToString()
    {
        string ls = "";
        foreach (var child in children)
        {
            ls += child.ToString() + ", ";
        }
        return $"PropListNode([{ls}])";
    }
}

public class PropDictionaryTailNode : PropNode
{
    protected override int GetDataType()
    {
        return PropNodeConst.DataTypeDictionaryTail;
    }
    
    public static PropDictionaryTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new PropDictionaryTailNode();
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropDictionaryTailNode.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        return;
    }

    public override string ToString()
    {
        return "PropDictionaryTailNode(null)";
    }
}

public class PropDictionaryNode<TKey> : PropBranchNode, IEnumerable<KeyValuePair<TKey, PropNode>>
{
    private Dictionary<TKey, PropNode> children = new Dictionary<TKey, PropNode>();
    private int keyType = PropNodeConst.DataTypeUndefined;

    public PropDictionaryNode()
    {
        SetKeyType();
    }
    public PropDictionaryNode(bool immutable_)
    {
        immutable = immutable_;
        SetKeyType();
    }

    private void SetKeyType()
    {
        if (typeof(TKey) == typeof(int)) keyType = PropNodeConst.DataTypeInt;
        else if (typeof(TKey) == typeof(float)) keyType = PropNodeConst.DataTypeFloat;
        else if (typeof(TKey) == typeof(string)) keyType = PropNodeConst.DataTypeString;
    }

    protected override int GetDataType()
    {
        return PropNodeConst.DataTypeDictionary;
    }

    #region REGION_DICTIONARY_API

    public PropNode this[TKey key]
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

    public bool TryGetValue(TKey key, out PropNode value)
    {
        return children.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<TKey, PropNode>> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator(); // Ĭ�Ϸ���KVP�汾��ö����
    }

    public void Add(TKey key, PropNode value)
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

    public static PropDictionaryNode<TKey> Deserialize(BinaryReader reader)
    {
        try
        {
            PropDictionaryNode<TKey> dictionary = new PropDictionaryNode<TKey>();
            while (true)
            {
                PropNode kNode = PropStreamer.Deserialize(reader);
                if (kNode is PropDictionaryTailNode tailNode) break;

                PropNode vNode = PropStreamer.Deserialize(reader);
                if (kNode is PropIntNode kIntNode && kIntNode.GetValue() is TKey ik)
                {
                    dictionary.Add(ik, vNode);
                }
                else if (kNode is PropFloatNode kFloatNode && kFloatNode.GetValue() is TKey fk)
                {
                    dictionary.Add(fk, vNode);
                }
                else if (kNode is PropStringNode kStringNode && kStringNode.GetValue() is TKey sk)
                {
                    dictionary.Add(sk, vNode);
                }
                else throw new InvalidDataException($"Key type {kNode.GetType()} does not match expected type {typeof(TKey)}.");
            }
            return dictionary;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropDictionaryNode.", ex);
        }
    }
    
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(dataType);
        writer.Write(keyType);
        foreach (var kvp in children)
        {
            TKey key = kvp.Key;
            PropNode value = kvp.Value;
            if (keyType == PropNodeConst.DataTypeInt)
            {
                if (key is int intKey)
                {
                    PropIntNode intNode = new PropIntNode(intKey);
                    intNode.Serialize(writer);
                }
                else throw new InvalidOperationException("Key type does not match expected type.");
            }
            else if (keyType == PropNodeConst.DataTypeFloat)
            {
                if (key is float floatKey)
                {
                    PropFloatNode floatNode = new PropFloatNode(floatKey);
                    floatNode.Serialize(writer);
                }
                else throw new InvalidOperationException("Key type does not match expected type.");
            }
            else if (keyType == PropNodeConst.DataTypeString)
            {
                if (key is string stringKey)
                {
                    PropStringNode stringNode = new PropStringNode(stringKey);
                    stringNode.Serialize(writer);
                }
                else throw new InvalidOperationException("Key type does not match expected type.");
            }
            else throw new InvalidOperationException($"Unsupported key type: {keyType}");
            value.Serialize(writer);
        }
        writer.Write(PropNodeConst.DataTypeDictionaryTail);
    }

    public override string ToString()
    {
        string ds = "";
        foreach (var kvp in children)
        {
            ds += $"{kvp.Key}: {kvp.Value.ToString()}, ";
        }
        return "PropDictionaryNode({" + $"{ds}" + "})";
    }
}

public class PropVector3Node : PropBranchNode
{
    private float x = 0f;
    private float y = 0f;
    private float z = 0f;
    
    public PropVector3Node() {}
    public PropVector3Node(float x_, float y_, float z_)
    {
        x = x_;
        y = y_;
        z = z_;
    }
    
    protected override int GetDataType()
    {
        return PropNodeConst.DataTypeVector3;
    }
    
    public static PropVector3Node Deserialize(BinaryReader reader)
    {
        try
        {
            return new PropVector3Node(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropVector3Node.", ex);
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
        return $"PropVector3Node({x},{y},{z})";
    }
}

public class PropPathNode : PropBranchNode
{
    private PropListNode pathPoints = new PropListNode();
    private PropListNode pathYaws = new PropListNode();
    private PropDictionaryNode<string> pathAlias = new PropDictionaryNode<string>();
    
    public PropPathNode() {}
    public PropPathNode(PropListNode points, PropListNode yaws, PropDictionaryNode<string> alias)
    {
        pathPoints = points;
        pathYaws = yaws;
        pathAlias = alias;
    }
    
    protected override int GetDataType()
    {
        return PropNodeConst.DataTypePath;
    }
    
    public void AddPath(float x, float y, float z, float w)
    {
        pathPoints.Add(new PropVector3Node(x, y, z));
        pathYaws.Add(new PropFloatNode(w));
    }
    
    public void AddPathAlias(string name, string alias)
    {
        pathAlias[name] = new PropStringNode(alias);
    }
    
    public static PropPathNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new PropPathNode(
                PropStreamer.Deserialize(reader) as PropListNode,
                PropStreamer.Deserialize(reader) as PropListNode,
                PropStreamer.Deserialize(reader) as PropDictionaryNode<string>
            );
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropPathNode.", ex);
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
        string s = "PropPathNode(";
        s += $"pathPoints: {pathPoints}, ";
        s += $"pathYaws: {pathYaws}";
        s += $"pathAlias: {pathAlias}";
        s += ")";
        return s;
    }
    
}

public class PropStreamer
{
    public static byte[] Serialize(PropNode node)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        node.Serialize(writer);
        byte[] data = stream.ToArray();
        return data;
    }
    
    public static PropNode Deserialize(BinaryReader reader)
    {
        try
        {
           int type = reader.ReadInt32();
           if (type == PropNodeConst.DataTypeInt)
           {
               return PropIntNode.Deserialize(reader);
           }
           else if (type == PropNodeConst.DataTypeFloat)
           {
               return PropFloatNode.Deserialize(reader);
           }
           else if (type == PropNodeConst.DataTypeString)
           {
               return PropStringNode.Deserialize(reader);
           }
           else if (type == PropNodeConst.DataTypeBool)
           {
               return PropBoolNode.Deserialize(reader);
           }
           else if (type == PropNodeConst.DataTypeList)
           {
               return PropListNode.Deserialize(reader);
           }
           else if (type == PropNodeConst.DataTypeListTail)
           {
               return PropListTailNode.Deserialize(reader);
           }
           else if (type == PropNodeConst.DataTypeDictionary)
           {
                int keyType = reader.ReadInt32();
                if (keyType == PropNodeConst.DataTypeInt) return PropDictionaryNode<int>.Deserialize(reader);
                else if (keyType == PropNodeConst.DataTypeFloat) return PropDictionaryNode<float>.Deserialize(reader);
                else if (keyType == PropNodeConst.DataTypeString) return PropDictionaryNode<string>.Deserialize(reader);
                else throw new InvalidDataException("Unsupported key type for dictionary deserialization.");
           }
           else if (type == PropNodeConst.DataTypeDictionaryTail)
           {
               return PropDictionaryTailNode.Deserialize(reader);
           }
           else if (type == PropNodeConst.DataTypeVector3)
           {
               return PropVector3Node.Deserialize(reader);
           }
           else if (type == PropNodeConst.DataTypePath)
           {
               return PropPathNode.Deserialize(reader);
           }
           else throw new InvalidDataException($"Unsupported data type: {type}");
        }
        catch
        {
            throw new InvalidDataException("Failed to deserialize PropNode.");
        }
    }

    public static PropDictionaryNode<string> SerializeInstance(object instance, int syncType)
    {
        PropDictionaryNode<string> dict = new PropDictionaryNode<string>();
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
                    dict.Add(property.Name, (PropNode)value);
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
                    dict.Add(field.Name, (PropNode)value);
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
        PropDictionaryNode<int> intD = new PropDictionaryNode<int>();
        intD.Add(1, new PropIntNode(1));
        intD.Add(2, new PropFloatNode(2f));
        intD.Add(3, new PropStringNode("3"));
        
        PropDictionaryNode<float> floatD = new PropDictionaryNode<float>();
        floatD[1.1f] = new PropIntNode(11);
        floatD[2.2f] = new PropFloatNode(22f);
        floatD[3.3f] = new PropStringNode("33");
        
        PropListNode l = new PropListNode();
        l.Add(floatD);
        intD[4] = l;

        PropDictionaryNode<string> stringD = new PropDictionaryNode<string>();
        stringD["a"] = new PropIntNode(111);
        stringD["b"] = new PropFloatNode(222f);
        stringD["c"] = new PropStringNode("333");
        intD[5] = stringD;
        
        intD[6] = new PropVector3Node(1f, 2f, 3f);
        
        PropPathNode path = new PropPathNode();
        path.AddPath(0.1f, 0.2f, 0.3f, 1.57f);
        path.AddPath(1.1f, 1.2f, 1.3f, 3.14f);
        path.AddPath(2.1f, 2.2f, 2.3f, 0.0f);
        path.AddPathAlias("n1", "a1");
        path.AddPathAlias("n2", "a2");
        path.AddPathAlias("n3", "a3");
        intD[7] = path;
        
        Console.WriteLine ($"intD is {intD}");
        
        byte[] bd = PropStreamer.Serialize(intD);
        using var stream = new MemoryStream(bd);
        using var reader = new BinaryReader(stream);
        PropNode intDD = PropStreamer.Deserialize(reader);
        Console.WriteLine ($"intDD is {intDD}");

        // Assert {intD} == {intDD}

        Console.WriteLine ("Over...");
    }   
}
*/
