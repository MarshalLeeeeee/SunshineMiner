using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : PlayerEntityBase
{
    public PlayerEntity() : base() { }
    public PlayerEntity(string eid) : base(eid) { }

    public override void InitFromDict(CustomDict baseProperty, CustomDict compProperty)
    {
        base.InitFromDict(baseProperty, compProperty);
        Debugger.Log($"Eid: {eid.Getter()}");
        Debugger.Log($"Eid: {eid.Getter()}");
        Debugger.Log($"Eid: {eid.Getter()}");
        Debugger.Log($"Eid: {eid.Getter()}");
        Debugger.Log($"Eid: {eid.Getter()}");
    }
}
