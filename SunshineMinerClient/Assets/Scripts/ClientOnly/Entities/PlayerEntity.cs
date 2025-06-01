using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerEntity : PlayerEntityBase
{
    public PlayerEntity() : base() { }
    public PlayerEntity(string eid) : base(eid) { }

    public override void Init()
    {
        base.Init();
        InitComponent("PrefabComp");
    }
}
