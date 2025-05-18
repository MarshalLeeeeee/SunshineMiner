using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PnlStartWidgetLogin : Widget
{
    [Serialize]
    public Button btnLogin;
    [Serialize]
    public HorizonInput inputAccount;

    protected override void Awake()
    {
        base.Awake();
        btnLogin.onClick.AddListener(OnBtnLoginClick);
    }

    protected override void SetActiveOnAwake()
    {
        if (activeOnAwake)
        {
            SetActive(activeOnAwake);
        }
        else
        {
            SetActive(Game.Instance.gate.IsConnected());
        }
    }

    private void OnBtnLoginClick()
    {
        Debug.Log($"{inputAccount}");
        Game.Instance.gate.Login(inputAccount.GetInput(), "");
    }
}
