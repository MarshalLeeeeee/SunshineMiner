using System.Collections;
using System.Collections.Generic;

public class ComponentFactoryBase
{
    static public Component? CreateComponent(string compName)
    {
        switch (compName)
        {
            case "AreaComp":
                return new AreaComp();
            default:
                return null;
        }
    }
}