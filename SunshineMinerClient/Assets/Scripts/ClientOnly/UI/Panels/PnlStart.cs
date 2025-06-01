using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlStart : Panel
{
    [SerializeField]
    private PnlStartWidgetWelcome widgetWelcome;
    private Guid delayDestroyTimer;

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.RemoveCover(); // destroy cover canvas
        widgetWelcome.Attach(OnClickWelcome);
    }

    private void OnEnable()
    {
        Game.Instance.eventManager.RegisterGlobalEvent<bool>("GateConnectingOver", "OnGateConnectingOver", OnGateConnectingOver);
        Game.Instance.eventManager.RegisterGlobalEvent<bool>("LoginRes", "OnLoginRes", OnLoginRes);
    }

    private void OnDisable()
    {
        Game.Instance.eventManager.UnregisterGlobalEvent("GateConnectingOver", "OnGateConnectingOver");
        Game.Instance.eventManager.UnregisterGlobalEvent("LoginRes", "OnLoginRes");
    }

    private void OnDestroy()
    {
        Game.Instance.timerManager.RemoveTimer(delayDestroyTimer);
    }

    private void OnGateConnectingOver(bool res)
    {
        if (res)
        {
            widgetWelcome.SetActive(false);
            if (widgets.TryGetValue("WidgetLogin", out var widget))
            {
                if (widget != null)
                {
                    PnlStartWidgetLogin widgetLogin = widget as PnlStartWidgetLogin;
                    widgetLogin.SetActive(true);
                }
            }
        }
    }

    private void OnLoginRes(bool res)
    {
        if (res)
        {
            delayDestroyTimer = Game.Instance.timerManager.AddTimer(100, SelfDestroy);
            
        }
    }

    private void SelfDestroy()
    {
        UIManager.Instance.UnloadPanel("PnlStart");
    }

    private void OnClickWelcome()
    {
        LoadWidgetAsync("WidgetLogin", "PnlStart/Login");
    }
}
