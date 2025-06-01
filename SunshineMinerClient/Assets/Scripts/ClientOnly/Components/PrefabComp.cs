using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabComp : Component
{
    private GameObject prefabObject = null;

    public override void OnLoad()
    {
        base.OnLoad();
        LoadPrefab("Assets/Models/Entities/Player.prefab");
    }

    public void LoadPrefab(string path)
    {
        Debugger.Log($"LoadPrefab {path}");
        ResourceManager.Instance.LoadResourceAsync(path, OnLoadPrefab);
    }

    private void OnLoadPrefab(GameObject gameObject)
    {
        prefabObject = ResourceManager.Instantiate(gameObject);
        prefabObject.transform.position = new Vector3(0,0,0);
    }
}
