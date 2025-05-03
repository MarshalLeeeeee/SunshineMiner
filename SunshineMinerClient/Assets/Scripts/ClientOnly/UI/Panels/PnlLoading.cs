using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PnlLoading : MonoBehaviour
{
    [SerializeField]
    private Image imgLoading;
    [SerializeField]
    private float speed;
    private bool increasing;

    // Start is called before the first frame update
    void Start()
    {
        imgLoading.type = Image.Type.Filled;
        imgLoading.fillMethod = Image.FillMethod.Radial360;
        imgLoading.fillAmount = 0f;
        imgLoading.fillClockwise = true;
        increasing = true;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        float fillAmount = imgLoading.fillAmount;
        bool fillClockwise = imgLoading.fillClockwise;

        if (increasing)
        {
            fillAmount += speed * dt;
            if (fillAmount > 1f)
            {
                fillAmount = 1f;
                increasing = false;
                fillClockwise = !fillClockwise;
            }
        }
        else
        {
            fillAmount -= speed * dt;
            if (fillAmount < 0f)
            {
                fillAmount = 0f;
                increasing = true;
                fillClockwise = !fillClockwise;
            }
        }

        imgLoading.fillAmount = fillAmount;
        imgLoading.fillClockwise = fillClockwise;
    }
}
