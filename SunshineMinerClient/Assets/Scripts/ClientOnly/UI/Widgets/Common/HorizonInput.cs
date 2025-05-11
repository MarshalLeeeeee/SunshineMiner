using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HorizonInput : Widget
{
    [Serialize]
    public TMP_InputField input;

    public string GetInput()
    {
        return input.text;
    }
}
