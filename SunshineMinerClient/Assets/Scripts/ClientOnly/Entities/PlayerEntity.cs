using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : PlayerEntityBase
{
    public PlayerEntity() : base() { }
    public PlayerEntity(string eid) : base(eid) { }

    public override void OnLoad()
    {
        base.OnLoad();
        LoadComponent<PrefabComp>("prefabComp");
    }
}
