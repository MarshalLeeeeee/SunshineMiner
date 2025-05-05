using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;

public class PnlStart : Panel
{
    [SerializeField]
    private PnlStartWidgetWelcome widgetWelcome;

    protected override void Awake()
    {
        base.Awake();
        UIManager.Instance.RemoveCover(); // destroy cover canvas
        widgetWelcome.Attach(OnClickWelcome);
    }

    private void OnEnable()
    {
        Game.Instance.eventManager.RegisterGlobalEvent<bool>("GateConnectingOver", "OnGateConnectingOver", OnGateConnectingOver);
    }

    private void OnDisable()
    {
        Game.Instance.eventManager.UnregisterGlobalEvent("GateConnectingOver", "OnGateConnectingOver");
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

    private void OnClickWelcome()
    {
        LoadWidgetAsync("WidgetLogin", "PnlStart/Login");
    }

    private void Update()
    {
        if (!widgets.TryGetValue("WidgetLogin", out var widget)) return;
        if (widget == null) return;
        PnlStartWidgetLogin widgetLogin = widget as PnlStartWidgetLogin;
        // widgetLogin.Test();
    }
}
