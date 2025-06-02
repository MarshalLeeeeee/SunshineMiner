using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEntity : PlayerEntityBase
{
    public PlayerEntity() : base() { }
    public PlayerEntity(string eid) : base(eid) { }

    protected override void InitComponents()
    {
        base.InitComponents();
        InitComponent<PrefabComp>("PrefabComp");
        InitComponent<AreaComp>("AreaComp");
    }
}
