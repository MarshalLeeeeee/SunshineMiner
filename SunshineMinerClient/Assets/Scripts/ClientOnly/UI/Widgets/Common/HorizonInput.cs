using TMPro;
using Unity.VisualScripting;

public class HorizonInput : Widget
{
    [Serialize]
    public TMP_InputField input;

    public string GetInput()
    {
        return input.text;
    }
}
