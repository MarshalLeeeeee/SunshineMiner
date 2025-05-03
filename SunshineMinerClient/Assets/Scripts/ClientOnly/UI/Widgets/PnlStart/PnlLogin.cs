using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlStartWidgetLogin : Widget
{
    protected override void SetActiveOnAwake()
    {
        if (activeOnAwake)
        {
            SetActive(activeOnAwake);
        }
        else
        {
            SetActive(Gate.Instance.IsConnected());
        }
    }

    public void Test()
    {
        Debug.Log("Test from PnlStartWidgetLogin");
    }
}
