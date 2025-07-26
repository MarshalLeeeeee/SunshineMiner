using System.Collections;
using System.Collections.Generic;

public class ComponentFactory
{
    public static Component? CreateComponent(string name)
    {
        switch (name)
        {
            case "AreaComp":
                return new AreaComp();
            default:
                return null;
        }
    }
}
