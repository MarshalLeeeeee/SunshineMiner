using System;
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
        Game.Instance.gate.ConnectedToServer();
        if (callback != null)
        {
            callback();
        }
    }
}
