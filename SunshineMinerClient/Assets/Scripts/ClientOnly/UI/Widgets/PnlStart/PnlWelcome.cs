using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PnlWelcome : MonoBehaviour
{
    private BgTouch bgTouch;

    // Start is called before the first frame update
    void Start()
    {
        bgTouch = transform.Find("BgTouchEnter").GetComponent<BgTouch>();
        bgTouch.AddClickCallback(ConnectToServer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ConnectToServer()
    {
        Gate.Instance.ConnectedToServer();
    }
}
