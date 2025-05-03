using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Widget : UIBase
{
    [SerializeField]
    protected bool activeOnAwake = true;

    protected override void Awake()
    {
        base.Awake();
        SetActiveOnAwake();
    }

    protected virtual void SetActiveOnAwake()
    {
        SetActive(activeOnAwake);
    }
}
