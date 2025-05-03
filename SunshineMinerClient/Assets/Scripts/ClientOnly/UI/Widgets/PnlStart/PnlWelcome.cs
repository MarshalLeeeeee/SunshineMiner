using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlStartWidgetWelcome : Widget
{
    [SerializeField]
    private BgTouch bgTouch;
    private Action callback;

    public void Attach(Action callback_)
    {
        callback = callback_;
        bgTouch.Attach(ConnectToServer);
    }

    private void ConnectToServer()
    {
        Gate.Instance.ConnectedToServer();
        if (callback != null)
        {
            callback();
        }
    }
}
