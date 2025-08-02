using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class PropNodeConst
{
    public const int TypeUndefined = 0;
    public const int TypeInt = 1;
    public const int TypeFloat = 2;
    public const int TypeString = 3;
    public const int TypeBool = 4;

    public static readonly int[] TypeLeaf = new int[]
    {
        TypeInt,
        TypeFloat,
        TypeString,
        TypeBool
    };

    public const int TypeList = 10;
    public const int TypeListTail = 11;

    public const int TypeIntDictionary = 20;
    public const int TypeFloatDictionary = 21;
    public const int TypeStringDictionary = 22;
    public const int TypeDictionaryTail = 23;
    
    public const int TypeVector3 = 101;
    public const int TypePath = 102;
}

public class PropNode
{
    /*
    * Parent node
    * For the root node, it is null.
    */
    public PropNode? parent = null;
    /*
    * Owner of this node, which is the func node.
    * For temporary nodes, it is null.
    */
    public FuncNode? owner = null;
    /*
    * Entity that this node belongs to.
    * Only when the owner is a Component, this will be non-null.
    */
    public Entity? entity
    {
        get
        {
            if (owner == null) return null;
            else if (owner is Component comp)
            {
                return comp.entity;
            }
            else return null;
        }
    }
    /*
    * Name of this node, holding by the owner.
    * Only the root node holds name.
    */
    public string name = "";
    /*
    * Name of the root node
    */
    public string rootName
    {
        get
        {
            if (parent == null) return name;
            else return parent.name;
        }
    }
    /*
    * Sync type of this node.
    * This is used to determine how this node should be synchronized.
    * Only the root node holds sync type.
    */
    public int syncType = SyncConst.Undefined;
    /*
    * Node hash, used for identification in the PropNode tree.
    * Node hash within all children should be unique. 
    */
    public string hash = "";
    public string fullHash
    {
        get
        {
            if (parent == null) return hash;
            else return parent.fullHash + "." + hash;
        }
    }

    /* static data type */
    public static int staticPropType = PropNodeConst.TypeUndefined;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public virtual int propType => staticPropType;

    public FuncNode? GetOwner()
    {
        if (parent == null) return owner;
        else return parent.GetOwner();
    }

    public Entity? GetEntity()
    {
        if (parent == null) return entity;
        else return parent.GetEntity();
    }

    public string GetName()
    {
        if (parent == null) return name;
        else return parent.GetName();
    }

    public int GetSyncType()
    {
        if (parent == null) return syncType;
        else return parent.GetSyncType();
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

    public virtual PropNode Copy()
    {
        return new PropNode();
    }

    public PropNode? GetNodeByHash(string propFullHash)
    {
        if (string.IsNullOrEmpty(propFullHash)) return this;
        string[] hashes = propFullHash.Split('.');
        return GetNodeByHashRecursive(hashes, 0);
    }

    public virtual PropNode? GetNodeByHashRecursive(string[] hashes, int index)
    {
        return null;
        
    }
}

/*
* PropLeafNode is a node that does not hold the reference of other PropNodes.
*/
public class PropLeafNode : PropNode
{
    public override PropNode? GetNodeByHashRecursive(string[] hashes, int index)
    {
        if (index == hashes.Length) return this;
        else return null;
    }
}

/*
* PropBranchNode is a node that holds the reference of other PropNodes.
*/
public class PropBranchNode : PropNode
{
    /*
    * This is the mapping from node hash to corresponding child prop node.
    */
    protected Dictionary<string, PropNode> hash2Children = new Dictionary<string, PropNode>();

    protected bool HasHash(string hash)
    {
        return hash2Children.ContainsKey(hash);
    }

    protected PropNode? GetChildByHash(string hash)
    {
        if (hash2Children.TryGetValue(hash, out PropNode? child))
        {
            return child;
        }
        return null;
    }

    protected void AddChildWithHash(PropNode? child)
    {
        if (child == null) return;
        string hash = child.hash;
        if (hash == "")
        {
            // TODO: implicit performance problem here!!!
            hash = Sampler.SampleGuid(4);
            while (HasHash(hash)) hash = Sampler.SampleGuid(4);
            child.hash = hash;
        }
        else
        {
            if (HasHash(hash))
            {
                Debugger.Log($"PropBranchNode AddChildWithHash: hash already exists: {hash}");
                return;
            }
        }
        DoAddChildWIthHash(child);
    }

    private void DoAddChildWIthHash(PropNode child)
    {
        string hash = child.hash;
        hash2Children.Add(hash, child);
        child.parent = this;
    }

    protected void RemoveChildWithHash(PropNode? child)
    {
        if (child == null) return;
        string hash = child.hash;
        if (!HasHash(hash)) return;
        hash2Children.Remove(hash);
    }

    protected void ClearChildWithHash()
    {
        hash2Children.Clear();
    }

    public override PropNode? GetNodeByHashRecursive(string[] hashes, int index)
    {
        if (index >= hashes.Length) return null;
        string hash = hashes[index];
        if (hash2Children.TryGetValue(hash, out PropNode propNode))
        {
            return propNode.GetNodeByHashRecursive(hashes, index + 1);
        }
        else return null;
    }
}

public class PropIntNode : PropLeafNode
{
    private int value = 0;

    public PropIntNode() {}
    public PropIntNode(int value_)
    {
        value = value_;
    }

    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeInt;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int value_)
    {
        int oldValue = value;
        if (oldValue == value_) return; // No change, no need to notify
        value = value_;
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
        writer.Write(PropNodeConst.TypeInt);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"PropIntNode({value})";
    }

    public override PropNode Copy()
    {
        return new PropIntNode(value);
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

    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeFloat;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;

    public float GetValue()
    {
        return value;
    }

    public void SetValue(float value_)
    {
        float oldValue = value;
        if (Math.Abs(oldValue - value_) < 0.0001f) return; // No change, no need to notify
        value = value_;
        Entity? e = GetEntity();
        if (e != null)
        {
            PropComp? propComp = e.GetComponent<PropComp>();
            if (propComp != null)
            {
                propComp.OnFloatSetter(oldValue, value, syncType, GetOwner(), rootName, fullHash);
            }
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
        writer.Write(PropNodeConst.TypeFloat);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"PropFloatNode({value})";
    }

    public override PropNode Copy()
    {
        return new PropFloatNode(value);
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

    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeString;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;

    public string GetValue()
    {
        return value;
    }

    public void SetValue(string value_)
    {
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
        writer.Write(PropNodeConst.TypeString);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"PropStringNode({value})";
    }

    public override PropNode Copy()
    {
        return new PropStringNode(value);
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

    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeBool;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;

    public bool GetValue()
    {
        return value;
    }

    public void SetValue(bool value_)
    {
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
        writer.Write(PropNodeConst.TypeBool);
        writer.Write(value);
    }

    public override string ToString()
    {
        return $"PropBoolNode({value})";
    }

    public override PropNode Copy()
    {
        return new PropBoolNode(value);
    }
}

public class PropListTailNode: PropNode
{
    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeListTail;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;
    
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

    public override PropNode Copy()
    {
        return new PropListTailNode();
    }
}

public class PropListNode : PropBranchNode, IEnumerable<PropNode>
{
    private List<PropNode> children = new List<PropNode>();

    public PropListNode() {}

    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeList;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;

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
            if (index < 0 || index >= children.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            PropNode oldValue = children[index];
            RemoveChildWithHash(oldValue);
            children[index] = value;
            AddChildWithHash(value);
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
        children.Add(child);
        AddChildWithHash(child);
    }

    public void Insert(int index, PropNode child)
    {
        if (index < 0 || index > children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        children.Insert(index, child);
        AddChildWithHash(child);
    }

    public void Remove(PropNode child)
    {
        children.Remove(child);
        RemoveChildWithHash(child);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        PropNode child = children[index];
        children.RemoveAt(index);
        RemoveChildWithHash(child);
    }

    public void Clear()
    {
        children.Clear();
        ClearChildWithHash();
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
        writer.Write(PropNodeConst.TypeList);
        foreach (var child in children)
        {
            child.Serialize(writer);
        }
        writer.Write(PropNodeConst.TypeListTail);
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

    public override PropNode Copy()
    {
        PropListNode copy = new PropListNode();
        foreach (PropNode child in children)
        {
            copy.Add(child.Copy());
        }
        return copy;
    }
}

public class PropDictionaryTailNode : PropNode
{
    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeDictionaryTail;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;
    
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

    public override PropNode Copy()
    {
        return new PropDictionaryTailNode();
    }
}

public class PropDictionaryNode<TKey> : PropBranchNode, IEnumerable<KeyValuePair<TKey, PropNode>>
{
    protected Dictionary<TKey, PropNode> children = new Dictionary<TKey, PropNode>();

    public PropDictionaryNode() { }

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
            if (children.TryGetValue(key, out PropNode? oldValue))
            {
                RemoveChildWithHash(oldValue);
            }
            children[key] = value;
            AddChildWithHash(value);
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
        children.Add(key, value);
        AddChildWithHash(value);
    }

    public void Remove(TKey key)
    {
        if (children.TryGetValue(key, out PropNode? value))
        {
            children.Remove(key);
            RemoveChildWithHash(value);
        }
    }

    public void Clear()
    {
        children.Clear();
        ClearChildWithHash();
    }

    #endregion

    public override string ToString()
    {
        string ds = "";
        foreach (var kvp in children)
        {
            ds += $"{kvp.Key}: {kvp.Value.ToString()}, ";
        }
        return "PropDictionaryNode({" + $"{ds}" + "})";
    }

    public override PropNode Copy()
    {
        PropDictionaryNode<TKey> copy = new PropDictionaryNode<TKey>();
        foreach (var kvp in children)
        {
            copy.Add(kvp.Key, kvp.Value.Copy());
        }
        return copy;
    }
}

public class PropIntDictionaryNode : PropDictionaryNode<int>
{
    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeIntDictionary;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;

    public static PropIntDictionaryNode Deserialize(BinaryReader reader)
    {
        try
        {
            PropIntDictionaryNode dictionary = new PropIntDictionaryNode();
            while (true)
            {
                PropNode kNode = PropStreamer.Deserialize(reader);
                if (kNode is PropDictionaryTailNode tailNode) break;

                PropNode vNode = PropStreamer.Deserialize(reader);
                if (kNode is PropIntNode kIntNode && kIntNode.GetValue() is int ik)
                {
                    dictionary.Add(ik, vNode);
                }
                else throw new InvalidDataException($"Key type {kNode.GetType()} does not match expected type {typeof(int)}.");
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
        writer.Write(PropNodeConst.TypeIntDictionary);
        foreach (var kvp in children)
        {
            int key = kvp.Key;
            PropIntNode intNode = new PropIntNode(key);
            intNode.Serialize(writer);
            PropNode value = kvp.Value;
            value.Serialize(writer);
        }
        writer.Write(PropNodeConst.TypeDictionaryTail);
    }
}

public class PropFloatDictionaryNode : PropDictionaryNode<float>
{
    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeFloatDictionary;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;

    public static PropFloatDictionaryNode Deserialize(BinaryReader reader)
    {
        try
        {
            PropFloatDictionaryNode dictionary = new PropFloatDictionaryNode();
            while (true)
            {
                PropNode kNode = PropStreamer.Deserialize(reader);
                if (kNode is PropDictionaryTailNode tailNode) break;

                PropNode vNode = PropStreamer.Deserialize(reader);
                if (kNode is PropFloatNode kFloatNode && kFloatNode.GetValue() is float fk)
                {
                    dictionary.Add(fk, vNode);
                }
                else throw new InvalidDataException($"Key type {kNode.GetType()} does not match expected type {typeof(float)}.");
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
        writer.Write(PropNodeConst.TypeFloatDictionary);
        foreach (var kvp in children)
        {
            float key = kvp.Key;
            PropFloatNode floatNode = new PropFloatNode(key);
            floatNode.Serialize(writer);
            PropNode value = kvp.Value;
            value.Serialize(writer);
        }
        writer.Write(PropNodeConst.TypeDictionaryTail);
    }
}

public class PropStringDictionaryNode : PropDictionaryNode<string>
{
    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeStringDictionary;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;

    public static PropStringDictionaryNode Deserialize(BinaryReader reader)
    {
        try
        {
            PropStringDictionaryNode dictionary = new PropStringDictionaryNode();
            while (true)
            {
                PropNode kNode = PropStreamer.Deserialize(reader);
                if (kNode is PropDictionaryTailNode tailNode) break;

                PropNode vNode = PropStreamer.Deserialize(reader);
                if (kNode is PropStringNode kStringNode && kStringNode.GetValue() is string sk)
                {
                    dictionary.Add(sk, vNode);
                }
                else throw new InvalidDataException($"Key type {kNode.GetType()} does not match expected type {typeof(string)}.");
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
        writer.Write(PropNodeConst.TypeStringDictionary);
        foreach (var kvp in children)
        {
            string key = kvp.Key;
            PropStringNode stringNode = new PropStringNode(key);
            stringNode.Serialize(writer);
            PropNode value = kvp.Value;
            value.Serialize(writer);
        }
        writer.Write(PropNodeConst.TypeDictionaryTail);
    }
}

public class PropVector3Node : PropLeafNode
{
    private float x = 0f;
    private float y = 0f;
    private float z = 0f;

    public PropVector3Node() { }
    public PropVector3Node(float x_, float y_, float z_)
    {
        x = x_;
        y = y_;
        z = z_;
    }

    /* static data type */
    public new static int staticPropType = PropNodeConst.TypeVector3;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;

    public static PropVector3Node Deserialize(BinaryReader reader)
    {
        try
        {
            return new PropVector3Node(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropVector3Node.", ex);
        }
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(PropNodeConst.TypeVector3);
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);
    }

    public override string ToString()
    {
        return $"PropVector3Node({x},{y},{z})";
    }

    public override PropNode Copy()
    {
        return new PropVector3Node(x, y, z);
    }
}

public class PropPathNode : PropBranchNode
{
    private PropListNode pathPoints = new PropListNode();
    private PropListNode pathYaws = new PropListNode();
    private PropStringDictionaryNode pathAlias = new PropStringDictionaryNode();
    
    public PropPathNode() {}
    public PropPathNode(PropListNode points, PropListNode yaws, PropStringDictionaryNode alias)
    {
        pathPoints = points;
        AddChildWithHash(pathPoints);
        pathYaws = yaws;
        AddChildWithHash(pathYaws);
        pathAlias = alias;
        AddChildWithHash(pathAlias);
    }
    
    /* static data type */
    public new static int staticPropType = PropNodeConst.TypePath;
    /*
    * Used for type checks of parameters of RPC methods.
    */
    public override int propType => staticPropType;
    
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
                PropStreamer.Deserialize(reader) as PropStringDictionaryNode
            );
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PropPathNode.", ex);
        }
    }
    
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(PropNodeConst.TypePath);
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
    
    public override PropNode Copy()
    {
        PropListNode pointsCopy = pathPoints.Copy() as PropListNode;
        PropListNode yawsCopy = pathYaws.Copy() as PropListNode;
        PropStringDictionaryNode aliasCopy = pathAlias.Copy() as PropStringDictionaryNode;
        return new PropPathNode(pointsCopy, yawsCopy, aliasCopy);
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
            MethodInfo? deserializeMethod = Factory.GetDeserializeMethod(type);
            if (deserializeMethod != null)
            {
                return (PropNode)deserializeMethod.Invoke(null, new object[] { reader });
            }
            else
            {
                throw new InvalidDataException($"Unsupported prop node type: {type}");
            }
        }
        catch
        {
            throw new InvalidDataException("Failed to deserialize PropNode.");
        }
    }

    public static PropStringDictionaryNode SerializeInstance(object instance, int syncType)
    {
        PropStringDictionaryNode dict = new PropStringDictionaryNode();
        Type type = instance.GetType();
        var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var property in properties)
        {
            var attr = property.GetCustomAttribute<PropertySyncAttribute>();

            if (attr != null && (attr.syncType & syncType) != 0)
            {
                var value = property.GetValue(instance);
                if (value != null && value is PropNode propNode)
                {
                    dict.Add(property.Name, propNode.Copy());
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
                if (value != null && value is PropNode propNode)
                {
                    dict.Add(field.Name, propNode.Copy());
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

        PropStringDictionaryNode stringD = new PropStringDictionaryNode();
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
