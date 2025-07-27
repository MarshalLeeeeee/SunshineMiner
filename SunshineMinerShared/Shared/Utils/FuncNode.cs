using System;
using System.Collections;
using System.Collections.Generic;

/*
* FuncNode is a base class for nodes that can 
*   contain the roots of data nodes
*   hold functions
*   and manage child func nodes.
*/
public class FuncNode
{
    protected Dictionary<string, FuncNode> funcNodes = new Dictionary<string, FuncNode>();
    public FuncNode? parent { get; protected set; } = null;

    /*
    * Get the full path of this FuncNode within the hierarchy.
    * The full path of the root is empty.
    * The full path of a child is the concatenation of the name of func node along the path (seperated by '.').
    */
    public string fullPath
    {
        get
        {
            if (parent == null) return "";
            else
            {
                string parentPath = parent.fullPath;
                if (string.IsNullOrEmpty(parentPath)) return GetType().Name;
                else return parentPath + "." + GetType().Name;
            }
        }
    }

    #region REGION_CHILDREN_MANAGEMENT

    /*
    * Get a FuncNode of type T
    * Returns null if not found or not of type T
    * 
    * @param T The type of FuncNode to retrieve
    * @return FuncNode of type T or null
    */
    public T? GetFuncNode<T>() where T : FuncNode
    {
        Type type = typeof(T);
        string name = type.Name;
        if (funcNodes.TryGetValue(name, out var funcNode))
        {
            if (funcNode != null && funcNode is T node)
            {
                return node;
            }
            else return null;
        }
        else return null;
    }

    /*
     * Get a FuncNode by name
     * Returns null if not found
     * 
     * @param name The name of the FuncNode to retrieve
     * @return FuncNode with the specified name or null
     */
    public FuncNode? GetFuncNodeByName(string name)
    {
        if (funcNodes.TryGetValue(name, out FuncNode funcNode))
        {
            if (funcNode != null)
            {
                return funcNode;
            }
            else return null;
        }
        else return null;
    }

    public FuncNode? GetFuncNodeByFullPath(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath)) return this;
        string[] paths = fullPath.Split('.');
        return GetFuncNodeByFullPathRecursive(paths, 0);
    }

    private FuncNode? GetFuncNodeByFullPathRecursive(string[] paths, int index)
    {
        if (index >= paths.Length) return this;
        string part = paths[index];
        if (funcNodes.TryGetValue(part, out FuncNode funcNode))
        {
            return funcNode.GetFuncNodeByFullPathRecursive(paths, index + 1);
        }
        else return null;
    }

    /*
    * Iterate through all FuncNodes
    */
    public IEnumerable<KeyValuePair<string, FuncNode>> IterFuncNodes()
    {
        foreach (KeyValuePair<string, FuncNode> kvp in funcNodes)
        {
            yield return kvp;
        }
    }

    public virtual void SetParent(FuncNode? p)
    {
        parent = p;
    }

    /*
     * Add a new FuncNode of type T
     * If a FuncNode with the same name already exists, it will be replaced
     * 
     * @param T The type of FuncNode to add
     * @return The newly added FuncNode of type T
     */
    public T AddFuncNode<T>(FuncNode? parent) where T : FuncNode, new()
    {
        Type type = typeof(T);
        string name = type.Name;
        if (funcNodes.TryGetValue(name, out FuncNode funcNode))
        {
            if (funcNode != null && funcNode is T tFuncNode)
            {
                return tFuncNode;
            }
        }
        funcNodes[name] = new T();
        funcNodes[name].SetParent(parent);
        return funcNodes[name] as T;
    }

    public void AddFuncNode(FuncNode? funcNode, FuncNode? parent)
    {
        if (funcNode == null) return;
        string name = funcNode.GetType().Name;
        if (funcNodes.ContainsKey(name)) return;
        funcNodes[name] = funcNode;
        funcNodes[name].SetParent(parent);
    }

    public void RemoveFuncNode<T>() where T : FuncNode
    {
        Type type = typeof(T);
        string name = type.Name;
        if (funcNodes.ContainsKey(name))
        {
            funcNodes.Remove(name);
        }
    }

    /*
     * Remove a FuncNode by name
     * 
     * @param name The name of the FuncNode to remove
     */
    public void RemoveFuncNodeByName(string name)
    {
        if (funcNodes.ContainsKey(name))
        {
            funcNodes.Remove(name);
        }
    }

    #endregion
}
