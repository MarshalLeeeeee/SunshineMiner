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

    private void OnClickWelcome()
    {
        LoadWidgetAsync("WidgetLogin", "PnlStart/Login");
    }

    private void Update()
    {
        if (!widgets.TryGetValue("WidgetLogin", out var widget)) return;
        if (widget == null) return;
        PnlStartWidgetLogin widgetLogin = widget as PnlStartWidgetLogin;
        widgetLogin.Test();
    }
}
