using UnityEngine;

public class PrefabComp : Component
{
    public GameObject? prefabObject = null;

    public override void Init(string eid)
    {
        base.Init(eid);
    }

    public override void OnLoad()
    {
        base.OnLoad();
        LoadPrefab("Assets/Models/Entities/Player.prefab");
    }

    public void LoadPrefab(string path)
    {
        ResourceManager.Instance.LoadResourceAsync(path, OnLoadPrefab);
    }

    private void OnLoadPrefab(GameObject gameObject)
    {
        prefabObject = ResourceManager.Instantiate(gameObject);
        Vector3 p = new Vector3(0, 0, 0);
        AreaComp? areaComp = GetComponent<AreaComp>("AreaComp");
        if (areaComp != null )
        {
            Vec3 areaPosition = areaComp.areaPosition;
            p = new Vector3(areaPosition.x, areaPosition.y, areaPosition.z);
        }
        prefabObject.transform.position = p;
        Entity? entity = this.entity;
        if (entity != null)
        {
            MoveBehavior moveBehavior = prefabObject.GetComponent<MoveBehavior>();
            moveBehavior.isPrimary = (entity.eid.Getter() == Game.Instance.entityManager.primaryPid);
        }
    }
}
